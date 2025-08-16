using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using webapi.Models;
using webapi.Repositories;

namespace webapi.Services
{
    // Interface
    public interface IForgotPasswordService
    {
        Task<(bool Success, string Message)> SendOtpAsync(ForgotPasswordRequestDto request);
        Task<(bool Success, string Message)> VerifyOtpAsync(VerifyOtpDto request);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto request);
    }

    // Implementation
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IForgotPasswordRepository _repository;
        private readonly IConfiguration _configuration;
        
        // In-memory storage for OTPs (for production, use Redis or database)
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime ExpiryTime)> _otpStore 
            = new ConcurrentDictionary<string, (string, DateTime)>();

        public ForgotPasswordService(IForgotPasswordRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message)> SendOtpAsync(ForgotPasswordRequestDto request)
        {
            try
            {
                // Check if email exists
                var emailExists = await _repository.CheckEmailExistsAsync(request.Email);
                if (!emailExists)
                {
                    return (false, "You don't exist, kindly signup.");
                }
 
                var otp = GenerateOtp();
                var expiryTime = DateTime.UtcNow.AddMinutes(10); 
                
                _otpStore.AddOrUpdate(request.Email, (otp, expiryTime), (key, value) => (otp, expiryTime));
 
                await SendEmailAsync(request.Email, otp);

                return (true, "OTP sent successfully to your email.");
            }
            catch (Exception ex)
            {
                return (false, $"Error sending OTP: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> VerifyOtpAsync(VerifyOtpDto request)
        {
            try
            {
                if (!_otpStore.TryGetValue(request.Email, out var storedOtpData))
                {
                    return (false, "OTP not found. Please request a new OTP.");
                }

                if (DateTime.UtcNow > storedOtpData.ExpiryTime)
                {
                    _otpStore.TryRemove(request.Email, out _);
                    return (false, "OTP has expired. Please request a new OTP.");
                }

                if (storedOtpData.Otp != request.Otp)
                {
                    return (false, "Invalid OTP. Please try again.");
                }

                return (true, "OTP verified successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error verifying OTP: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto request)
        {
            try
            { 
                var otpVerification = await VerifyOtpAsync(new VerifyOtpDto 
                { 
                    Email = request.Email, 
                    Otp = request.Otp 
                });

                if (!otpVerification.Success)
                {
                    return otpVerification;
                }

                // Hash the new password
                var hashedPassword = HashPassword(request.NewPassword);

                // Update password in database
                var updateResult = await _repository.ResetPasswordAsync(request.Email, hashedPassword);

                if (updateResult)
                {
                    // Remove OTP from store after successful password reset
                    _otpStore.TryRemove(request.Email, out _);
                    return (true, "Password reset successfully.");
                }
                else
                {
                    return (false, "Failed to reset password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error resetting password: {ex.Message}");
            }
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private async Task SendEmailAsync(string toEmail, string otp)
        {
            var smtpHost = _configuration["SmtpSettings:Host"];
            var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
            var fromEmail = _configuration["SmtpSettings:Email"];
            var fromPassword = _configuration["SmtpSettings:Password"];
            Console.WriteLine($"smtpHost: {_configuration["SmtpSettings:Host"]}");
            Console.WriteLine($"smtpPort: {_configuration["SmtpSettings:Port"]}");
            Console.WriteLine($"fromEmail: {_configuration["SmtpSettings:Email"]}");
            Console.WriteLine($"fromPassword: {_configuration["SmtpSettings:Password"]}");

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Password Reset OTP",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Password Reset Request</h2>
                            <p>You have requested to reset your password. Please use the following OTP:</p>
                            <h3 style='color: #007bff; font-size: 24px;'>{otp}</h3>
                            <p>This OTP will expire in 10 minutes.</p>
                            <p>If you didn't request this, please ignore this email.</p>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);
                await client.SendMailAsync(message);
            }
        }
    }
}