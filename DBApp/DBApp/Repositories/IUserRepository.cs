using DBApp.Models;

namespace DBApp.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}