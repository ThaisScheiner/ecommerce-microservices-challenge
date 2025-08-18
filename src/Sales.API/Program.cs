using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Sales.API.Data;
using Sales.API.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration));

// Adicionar o DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Configurar Autenticação JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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

// ## A CORREÇÃO ESTÁ AQUI ##
// Configurar o HttpClient para chamar o Stock.API
builder.Services.AddHttpClient("StockService", client =>
{
    // Esta linha garante que a BaseAddress seja lida do appsettings.json
    string stockServiceUrl = builder.Configuration["StockServiceUrl"]
        ?? throw new InvalidOperationException("StockServiceUrl is not configured.");

    client.BaseAddress = new Uri(stockServiceUrl);
});

builder.Services.AddSingleton<IMessageBusClient, RabbitMQClient>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ... (resto do código igual)

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();