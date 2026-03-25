using TaskManagerApi.Services;
using TaskManagerApi.DTOs;
using TaskManagerApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")] // Isso define a rota como /api/auth
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Procurar colaborador pelo e-mail
        var colaborador = await _context.Colaboradores.FirstOrDefaultAsync(x => x.Email == request.Email);

        // Validar se o usuário existe e se a senha está correta de acordo com o Banco de Dados(por enquanto texto puro no banco)
        if(colaborador == null || colaborador.Senha != request.Senha)
        {
            return Unauthorized(new { message = "E-mail ou senha inválidos!"});
        }

        // Gerar Token
        var token = _tokenService.GerarToken(colaborador);

        // Retornar o nome e o token para Front-End/Postman/Scalar
        return Ok(new
        {
            usuario = colaborador.Nome,
            token = token
        });
    }
}