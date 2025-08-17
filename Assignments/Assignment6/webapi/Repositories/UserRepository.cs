using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Users WHERE Email = @email";
                
                var parameter = new SqlParameter("@email", SqlDbType.NVarChar, 100) { Value = email };
                command.Parameters.Add(parameter);
                
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        UserRole = reader.GetString("UserRole"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    };
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving user by email: {ex.Message}", ex);
            }
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetUserById";
                
                var parameter = new SqlParameter("@UserId", SqlDbType.Int) { Value = userId };
                command.Parameters.Add(parameter);
                
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        UserRole = reader.GetString("UserRole"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    };
                }
                
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving user by ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var users = new List<User>();
                
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllUsers";
                
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        UserId = reader.GetInt32("UserId"),
                        Email = reader.GetString("Email"),
                        Password = reader.GetString("Password"),
                        UserRole = reader.GetString("UserRole"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    });
                }
                
                return users;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all users: {ex.Message}", ex);
            }
        }

public async Task<int> AddAsync(User user)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        using var command = new SqlCommand("stp_AddUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Password", user.Password);
        command.Parameters.AddWithValue("@UserRole", user.UserRole);
 
        var userIdParam = new SqlParameter("@NewUserId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(userIdParam);

        await command.ExecuteNonQueryAsync();
         
        if (userIdParam.Value == null || userIdParam.Value == DBNull.Value)
        {
            throw new InvalidOperationException("Failed to create user - no ID returned from database.");
        }
        
        return Convert.ToInt32(userIdParam.Value);
    }
    catch (SqlException ex)
    { 
        switch (ex.Number)
        {
            case 2627:  
            case 2601:  
                throw new InvalidOperationException("Email already exists.", ex);
            case 547:  
                throw new InvalidOperationException("Invalid reference data.", ex);
            default:
                throw new InvalidOperationException($"Database error occurred while adding user: {ex.Message}", ex);
        }
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error adding user: {ex.Message}", ex);
    }
}

        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_UpdateUser";
                
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = user.UserId });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = user.Email });
                command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 255) { Value = user.Password });
                command.Parameters.Add(new SqlParameter("@UserRole", SqlDbType.NVarChar, 20) { Value = user.UserRole });
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating user: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_DeleteUser";
                
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
                
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting user: {ex.Message}", ex);
            }
        }
    }
}