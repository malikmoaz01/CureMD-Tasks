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

                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT SCOPE_IDENTITY()";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding activity log: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int logId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_DeleteActivityLog";

                command.Parameters.Add(new SqlParameter("@LogId", SqlDbType.Int) { Value = logId });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting activity log: {ex.Message}", ex);
            }
        }
    }
}