using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticAssets;
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
    public async Task<IActionResult> ListarMinhasTarefas([FromQuery] string? status)
    {
        // Autenticação
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();
        var colaboradorId = int.Parse(userIdClaim.Value);

        // Cria consulta Query, mas ainda não bate no banco
        var query = _context.Tarefas
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

        query = query.OrderByDescending(t => t.DataCriacao);

        // O FILTRO: só trazer o que pertence ao coloborador
        var tarefas = await query
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

        return Ok(tarefas);
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