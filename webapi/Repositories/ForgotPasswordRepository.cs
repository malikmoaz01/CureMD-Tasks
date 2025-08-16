using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace webapi.Repositories
{
    // Interface
    public interface IForgotPasswordRepository
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string hashedPassword);
    }

    // Implementation
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        private readonly string _connectionString;

        public ForgotPasswordRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("stp_CheckEmailExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);

                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    
                    return Convert.ToInt32(result) == 1;
                }
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string hashedPassword)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("stp_ResetPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@NewPassword", hashedPassword);

                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    
                    return Convert.ToInt32(result) == 1;
                }
            }
        }
    }
}