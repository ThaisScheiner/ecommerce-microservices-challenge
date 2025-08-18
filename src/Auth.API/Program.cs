using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Serilog 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();


// Adiciona o middleware do Serilog para logar requisições HTTP
app.UseSerilogRequestLogging();


//  Mapeando os endpoints
app.MapPost("/login", (UserLogin user, IConfiguration config) =>
{
    if (user.Username == "admin" && user.Password == "password")
    {
        var secretKey = Encoding.ASCII.GetBytes(config["Jwt:Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = config["Jwt:Issuer"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new { token = tokenHandler.WriteToken(token) });
    }
    return Results.Unauthorized();
});

app.Run();

public record UserLogin(string Username, string Password);