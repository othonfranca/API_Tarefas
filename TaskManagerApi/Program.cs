using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Services;
using TaskManagerApi.Middleware;
using System.Text;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var secretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("A chave secreta para JWT não foi configurada.");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddOpenApi(options =>
{
    // Adicionamos o transformador que criamos para o "Cadeado"
    options.AddDocumentTransformer<SecurityTransformer>();
});

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

//Interface da API
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Gera o JSON do contrato
    app.MapScalarApiReference(options =>
    {
        options
        .WithTitle("Task Manager API")
        .WithTheme(ScalarTheme.DeepSpace)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    }); // Interface moderna em /scalar/v1
}


app.UseAuthentication(); // quem é o usuário?
app.UseAuthorization(); // o que o usuário pode acessar?

app.MapControllers(); // mapeia os controllers para as rotas

app.Run();