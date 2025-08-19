using Blazored.LocalStorage;
using Ecommerce.WebApp;
using Ecommerce.WebApp.Auth;
using Ecommerce.WebApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;
using Blazored.Toast;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registra nosso interceptador para que ele possa ser injetado
builder.Services.AddScoped<AuthHeaderHandler>();

// Configura o HttpClient para usar o interceptador
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri("http://localhost:5116"))
    .AddHttpMessageHandler<AuthHeaderHandler>();

// Necessário para podermos injetar o HttpClient normalmente nos nossos serviços
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

// Adiciona os serviços de autenticação
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Adiciona o serviço de produtos
builder.Services.AddScoped<IProductService, ProductService>();

// Adiciona o serviço de pedidos
builder.Services.AddScoped<IOrderService, OrderService>();

//Toast
builder.Services.AddBlazoredToast();

// Define a cultura padrão da aplicação para Português do Brasil (pt-BR)
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

await builder.Build().RunAsync();