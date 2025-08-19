namespace Ecommerce.WebApp.Services
{
    public interface IAuthService
    {
        Task<bool> Login(LoginModel loginModel);
        Task Logout();
    }

    public class LoginModel
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public record LoginResult(string Token);
}