using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Services;
using TaskManagerApi.Enums;
using TaskManagerApi.Models;
using TaskManagerApi.Middleware;
using System.Text;
using FluentValidation;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var secretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("A chave secreta para JWT não foi configurada.");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Aceita String ou Enum
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Deixa de ser sensitive case
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

//Midlewares
app.UseMiddleware<ExceptionMiddleware>();


app.UseAuthentication(); // quem é o usuário?
app.UseAuthorization(); // o que o usuário pode acessar?

app.MapControllers(); // mapeia os controllers para as rotas

app.Run();