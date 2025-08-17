using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IActivityLogRepository
    {
        Task<ActivityLog> GetByIdAsync(int logId);
        Task<IEnumerable<ActivityLog>> GetAllAsync();
        Task<int> AddAsync(ActivityLog activityLog);
        Task<bool> DeleteAsync(int logId);
    }

    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ActivityLogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ActivityLog> GetByIdAsync(int logId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetActivityLogById";

                command.Parameters.Add(new SqlParameter("@LogId", SqlDbType.Int) { Value = logId });

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ActivityLog
                    {
                        LogId = reader.GetInt32("LogId"),
                        LogDateTime = reader.GetDateTime("LogDateTime"),
                        Action = reader.GetString("Action"),
                        Success = reader.GetBoolean("Success"),
                        Details = reader.IsDBNull("Details") ? null : reader.GetString("Details"),
                        UserId = reader.IsDBNull("UserId") ? null : reader.GetInt32("UserId"),
                        VisitId = reader.IsDBNull("VisitId") ? null : reader.GetInt32("VisitId")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                throw new InvalidOperationException($"Error retrieving activity log: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetAllAsync()
        {
            try
            {
                var activityLogs = new List<ActivityLog>();

                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllActivityLogs";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    activityLogs.Add(new ActivityLog
                    {
                        LogId = reader.GetInt32("LogId"),
                        LogDateTime = reader.GetDateTime("LogDateTime"),
                        Action = reader.GetString("Action"),
                        Success = reader.GetBoolean("Success"),
                        Details = reader.IsDBNull("Details") ? null : reader.GetString("Details"),
                        UserId = reader.IsDBNull("UserId") ? null : reader.GetInt32("UserId"),
                        VisitId = reader.IsDBNull("VisitId") ? null : reader.GetInt32("VisitId")
                    });
                }

                return activityLogs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                throw new InvalidOperationException($"Error retrieving all activity logs: {ex.Message}", ex);
            }
        }

        public async Task<int> AddAsync(ActivityLog activityLog)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_AddActivityLog";

                command.Parameters.Add(new SqlParameter("@Action", SqlDbType.NVarChar, 100) { Value = activityLog.Action });
                command.Parameters.Add(new SqlParameter("@Success", SqlDbType.Bit) { Value = activityLog.Success });
                command.Parameters.Add(new SqlParameter("@Details", SqlDbType.NVarChar, 500) { Value = activityLog.Details ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = activityLog.UserId ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@VisitId", SqlDbType.Int) { Value = activityLog.VisitId ?? (object)DBNull.Value });

                var result = await command.ExecuteScalarAsync();
                
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                
                throw new InvalidOperationException("Failed to get the inserted activity log ID");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}");
                throw new InvalidOperationException($"Error adding activity log: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int logId)
{
    try
    {
        Console.WriteLine($"Repository: Attempting to delete activity log ID: {logId}");
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        Console.WriteLine("Repository: Database connection opened");

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_DeleteActivityLog";

        command.Parameters.Add(new SqlParameter("@LogId", SqlDbType.Int) { Value = logId });
        Console.WriteLine($"Repository: Executing delete for LogId: {logId}");
 
        var result = await command.ExecuteScalarAsync();
        Console.WriteLine($"Repository: Result from stored procedure: {result}");
        
        if (result != null && result != DBNull.Value)
        {
            int rowsAffected = Convert.ToInt32(result);
            Console.WriteLine($"Repository: Rows affected: {rowsAffected}");
            return rowsAffected > 0;
        }
        
        return false;
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Repository SQL Error in DeleteAsync: {ex.Message}");
        
        if (ex.Number == 50022)
            throw new ArgumentException("Activity log not found.");
            
        throw new InvalidOperationException($"Database error deleting activity log: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Repository Error in DeleteAsync: {ex.Message}");
        Console.WriteLine($"Repository Stack trace: {ex.StackTrace}");
        throw new InvalidOperationException($"Error deleting activity log: {ex.Message}", ex);
    }
}
    }
}