using DBApp.Models;

namespace DBApp.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(SignupDto signupDto);
        Task<bool> EmailExistsAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}