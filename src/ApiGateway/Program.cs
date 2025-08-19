using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ## PASSO 1: Adicionar a pol�tica de CORS ##
// Define uma pol�tica chamada "CorsPolicy"
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        // Permite que a aplica��o rodando em localhost:5069 fa�a chamadas
        policy.WithOrigins("http://localhost:5069")
              .AllowAnyMethod() // Permite qualquer m�todo (POST, GET, etc.)
              .AllowAnyHeader(); // Permite qualquer cabe�alho
    });
});

// Adiciona os servi�os do Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// Configura��o da Autentica��o JWT
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

// ## PASSO 2: Usar a pol�tica de CORS ##
// IMPORTANTE: Deve vir antes de app.UseOcelot()
app.UseCors("CorsPolicy");

// Middleware do Ocelot
await app.UseOcelot();

app.Run();