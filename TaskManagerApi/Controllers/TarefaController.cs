using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs;
using TaskManagerApi.Enums;
using TaskManagerApi.Models;
using TaskManagerApi.Validators;


namespace TaskManagerApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TarefaController : ControllerBase
{
    private readonly AppDbContext _context;

    public TarefaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ListarMinhasTarefas([FromQuery] string? status, [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
    {
        // Autenticação
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var colaboradorId = int.Parse(userIdClaim.Value);

        // Cria consulta Query, mas ainda não bate no banco
        var query = _context.Tarefas
            .AsNoTracking()
            .Where(t => t.ColaboradorId == colaboradorId)
            .AsQueryable();

        // Se o usuário passou um status na URL, adicionamos mais um filtro na Query
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<StatusTarefa>(status, ignoreCase: true, out var statusFiltro))
            {
                query = query.Where(t => t.Status == statusFiltro);
            }
            else
            {
                return BadRequest("Status invalido para filtro!");
            }
        }

        var totalRegistros = await query.CountAsync();

        // O FILTRO: só trazer o que pertence ao coloborador
        var tarefas = await query
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(t => new TarefaResponse
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                Status = t.Status.ToString(),
                DataCriacao = t.DataCriacao,
                NomeColaborador = t.Colaborador!.Nome
            })
            .ToListAsync();

        var response = new PaginatedResponse<TarefaResponse>
        {
            PaginaAtual = pagina,
            ItensPorPagina = tamanho,
            TotalRegistros = totalRegistros,
            Dados = tarefas
        };

        return Ok(response);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var colaboradorId = int.Parse(userIdClaim.Value);

        var stats = await _context.Tarefas
            .Where(t => t.ColaboradorId == colaboradorId)
            .GroupBy(t => 1) //agrupa tudo em um unico bloco
            .Select(g => new
            {
                Total = g.Count(),
                Concluidas = g.Count(t => t.Status == StatusTarefa.Concluido),
                EmAndamento = g.Count(t => t.Status == StatusTarefa.EmAndamento),
                Pendente = g.Count(t => t.Status == StatusTarefa.Pendente)
            })
            .FirstOrDefaultAsync();

        if (stats == null)
        {
            return Ok(new DashboardResponse()); // Retorna tudo zerado (padrão do DTO)
        }

        var response = new DashboardResponse
        {
            TotalTarefas = stats.Total,
            TarefasConcluidas = stats.Concluidas,
            PorcentagemConcluido = stats.Total > 0 ? Math.Round((double)stats.Concluidas / stats.Total * 100, 2) : 0,
            TarefasEmAndamento = stats.EmAndamento,
            PorcentagemEmAndamento = stats.Total > 0 ? Math.Round((double)stats.EmAndamento / stats.Total * 100, 2) : 0,
            TarefasPendentes = stats.Pendente
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CriarTarefa([FromBody] Tarefa tarefa)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim == null) return Unauthorized();

        tarefa.ColaboradorId = int.Parse(userIdClaim.Value);
        tarefa.DataCriacao = DateTime.Now;

        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();

        var nomeColaborador = await _context.Colaboradores
            .Where(c => c.Id == tarefa.ColaboradorId)
            .Select(c => c.Nome)
            .FirstOrDefaultAsync();

        var response = new TarefaResponse
        {
            Id = tarefa.Id,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            Status = tarefa.Status.ToString(),
            DataCriacao = tarefa.DataCriacao,
            NomeColaborador = nomeColaborador ?? "Sem nome no cadastro"
        };

        return CreatedAtAction(nameof(ListarMinhasTarefas), new {id = tarefa.Id}, response);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> AlterarStatusTarefa([FromRoute]int id, [FromBody] TarefaUpdateRequest dto)
    {
        // Autenticação - Usuário logado? - Via Token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var colaboradorId = int.Parse(userIdClaim.Value);

        // Validação dos dados inputados
        var validator = new TarefaUpdateRequestValidator();
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        // Busca e verifica se a tarefa existe e se é uma das tarefas do colaborador
        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.ColaboradorId == colaboradorId);

        if (tarefa == null) return NotFound("Tarefa não encontrada ou acesso negado!");

        // Execução

        if (!Enum.TryParse<StatusTarefa>(dto.Status, ignoreCase: true, out var statusConvertido))
        {
            return BadRequest("Falha ao converter status.");
        }
        tarefa.Status = statusConvertido;
        tarefa.DataAtualizacao = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(tarefa);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarTarefa(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var colaboradorId = int.Parse(userIdClaim.Value);

        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.ColaboradorId == colaboradorId);

        if (tarefa == null) return NotFound("Tarefa não encontrada ou acesso negado!");

        // Execução

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
}