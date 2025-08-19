using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ## PASSO 1: Adicionar a política de CORS ##
// Define uma política chamada "CorsPolicy"
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        // Permite que a aplicação rodando em localhost:5069 faça chamadas
        policy.WithOrigins("http://localhost:5069")
              .AllowAnyMethod() // Permite qualquer método (POST, GET, etc.)
              .AllowAnyHeader(); // Permite qualquer cabeçalho
    });
});

// Adiciona os serviços do Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// Configuração da Autenticação JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = false
        };
    });

var app = builder.Build();

// ## PASSO 2: Usar a política de CORS ##
// IMPORTANTE: Deve vir antes de app.UseOcelot()
app.UseCors("CorsPolicy");

// Middleware do Ocelot
await app.UseOcelot();

app.Run();