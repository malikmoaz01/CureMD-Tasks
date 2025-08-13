using DBApp.Models;
using DBApp.Repositories;
using Org.BouncyCastle.Crypto.Generators;

namespace DBApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<User> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            
            if (user != null && VerifyPassword(loginDto.Password, user.Password))
            {
                return user;
            }
            
            return null;
        }
        
        public async Task<bool> RegisterAsync(SignupDto signupDto)
        {
            if (await _userRepository.EmailExistsAsync(signupDto.Email))
            {
                return false;
            }
            
            var user = new User
            {
                Name = signupDto.Name,
                Email = signupDto.Email,
                Password = HashPassword(signupDto.Password),
                Role = signupDto.Role
            };
            
            return await _userRepository.CreateUserAsync(user);
        }
        
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}