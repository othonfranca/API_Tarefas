using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration) => _configuration = configuration;
    
    public string GerarToken(Colaborador colaborador)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("A chave secreta para JWT não foi configurada."));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.Name, colaborador.Nome),
                new Claim(ClaimTypes.Email, colaborador.Email),
                new Claim(ClaimTypes.NameIdentifier, colaborador.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }
}