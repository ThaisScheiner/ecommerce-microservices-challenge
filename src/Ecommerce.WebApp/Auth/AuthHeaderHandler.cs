using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Ecommerce.WebApp.Auth
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Verifica se já temos um token salvo
            if (await _localStorage.ContainKeyAsync("authToken"))
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                // Adiciona o cabeçalho de autorização na requisição
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}