using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs;
using TaskManagerApi.Models;


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
    public async Task<IActionResult> ListarMinhasTarefas()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim == null) return Unauthorized();

        var colaboradorId = int.Parse(userIdClaim.Value);

        // O FILTRO: só trazer o que pertence ao coloborador
        var tarefas = await _context.Tarefas
            .Where(t => t.ColaboradorId == colaboradorId)
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
}