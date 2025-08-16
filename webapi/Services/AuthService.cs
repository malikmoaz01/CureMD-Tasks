using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        string GenerateJwtToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    throw new ArgumentException("Email and password are required.");
                }

                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                
                if (user == null || !VerifyPassword(loginDto.Password, user.Password))
                {
                    throw new UnauthorizedAccessException("Invalid credentials.");
                }

                var token = GenerateJwtToken(user);

                return new AuthResponseDto
                {
                    Token = token,
                    UserRole = user.UserRole,
                    Email = user.Email,
                    UserId = user.UserId
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Login failed: {ex.Message}", ex);
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registerDto.Email) || 
                    string.IsNullOrWhiteSpace(registerDto.Password) ||
                    string.IsNullOrWhiteSpace(registerDto.UserRole))
                {
                    throw new ArgumentException("All fields are required for registration.");
                }

                if (!IsValidUserRole(registerDto.UserRole))
                {
                    throw new ArgumentException("Invalid user role. Must be 'Admin' or 'Receptionist'.");
                }

                var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists.");
                }

                var hashedPassword = HashPassword(registerDto.Password);

                var user = new User
                {
                    Email = registerDto.Email.Trim(),
                    Password = hashedPassword,
                    UserRole = registerDto.UserRole.Trim()
                };

                var userId = await _userRepository.AddAsync(user);
                return userId > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Registration failed: {ex.Message}", ex);
            }
        }

        public string GenerateJwtToken(User user)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JWT secret key is not configured.");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.UserRole),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiryInHours"])),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Token generation failed: {ex.Message}", ex);
            }
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
            }
            catch (Exception)
            {
                return inputPassword == hashedPassword;
            }
        }

        private string HashPassword(string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
            catch (Exception)
            {
                return password;
            }
        }

        private bool IsValidUserRole(string userRole)
        {
            return userRole == "Admin" || userRole == "Receptionist";
        }
    }
}