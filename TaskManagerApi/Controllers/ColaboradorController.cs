using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs;
using TaskManagerApi.Enums;
using TaskManagerApi.Models;
using TaskManagerApi.Validators;
using System.Security.Claims;

namespace TaskManagerApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ColaboradorController : ControllerBase
{
    private readonly AppDbContext _context;

    public ColaboradorController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ColaboradorController>>> ListarColaboradores([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
    {
        var skip = ((pagina - 1) * tamanho);

        var colaboradores = await _context.Colaboradores
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Skip(skip)
            .Take(tamanho)
            .Select(c => new ColaboradorResponse
            {
                Id = c.Id,
                Nome = c.Nome,
                Email = c.Email
            })
            .ToListAsync();

            return Ok(colaboradores);
    }

    [HttpPost]
    public async Task<ActionResult<ColaboradorResponse>> IncluirColaborador(Colaborador colaborador)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();

        _context.Colaboradores.Add(colaborador);
        await _context.SaveChangesAsync();

        var response = new ColaboradorResponse
        {
            Id = colaborador.Id,
            Nome = colaborador.Nome,
            Email = colaborador.Email
        };

        return CreatedAtAction(nameof(ListarColaboradores), new {id = colaborador.Id}, response);
    }

    [HttpPut("{id}/editarColab")]
    public async Task<ActionResult<ColaboradorResponse>> AlterarColaborador([FromRoute]int id, [FromBody] ColaboradorUpdateRequest dto)
    {
        // authentication
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
        var colaboradorId = int.Parse(userIdClaim.Value);

        if(colaboradorId != id) return Forbid();

        var colaboradorBanco = await _context.Colaboradores.FindAsync(id);

        if (colaboradorBanco == null) return NotFound("Colaborador não encontrado na base de dados!");

        //Execucao
        if(dto.Nome != null) colaboradorBanco.Nome = dto.Nome;
        if(dto.Email != null) colaboradorBanco.Email = dto.Email;
        if(dto.Senha != null) colaboradorBanco.Senha = dto.Senha;

        var response = new ColaboradorResponse
        {
            Id = colaboradorBanco.Id,
            Nome = colaboradorBanco.Nome,
            Email = colaboradorBanco.Email
        };

        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarColaborador([FromRoute]int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if(userIdClaim == null) return Unauthorized();

        var colaborador = await _context.Colaboradores.FindAsync(id);
        if(colaborador == null) return NotFound("Colaborador não encontrado na base de dados");

        _context.Colaboradores.Remove(colaborador);
        await _context.SaveChangesAsync();
    
        return NoContent();
    }

}