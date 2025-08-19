using Blazored.LocalStorage;
using Ecommerce.WebApp.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace Ecommerce.WebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<bool> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("/gateway/login", loginModel);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (loginResult?.Token == null)
            {
                return false;
            }

            // Salva o token no Local Storage do navegador
            await _localStorage.SetItemAsync("authToken", loginResult.Token);

            // Notifica a aplicação que o estado de autenticação mudou
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}