using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IVisitTypeRepository
    {
        Task<VisitType> GetByIdAsync(int visitTypeId);
        Task<IEnumerable<VisitType>> GetAllAsync();
        Task<int> AddAsync(VisitType visitType);
        Task<bool> UpdateAsync(VisitType visitType);
        Task<bool> DeleteAsync(int visitTypeId);
    }

    public class VisitTypeRepository : IVisitTypeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public VisitTypeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<VisitType> GetByIdAsync(int visitTypeId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetVisitTypeById";

                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = visitTypeId });

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new VisitType
                    {
                        VisitTypeId = reader.GetInt32("VisitTypeId"),
                        VisitTypeName = reader.GetString("VisitTypeName"),
                        BaseRate = reader.GetDecimal("BaseRate"),
                        Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving visit type: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<VisitType>> GetAllAsync()
        {
            try
            {
                var visitTypes = new List<VisitType>();

                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllVisitTypes";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    visitTypes.Add(new VisitType
                    {
                        VisitTypeId = reader.GetInt32("VisitTypeId"),
                        VisitTypeName = reader.GetString("VisitTypeName"),
                        BaseRate = reader.GetDecimal("BaseRate"),
                        Description = reader.IsDBNull("Description") ? null : reader.GetString("Description")
                    });
                }

                return visitTypes;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all visit types: {ex.Message}", ex);
            }
        }

        public async Task<int> AddAsync(VisitType visitType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_AddVisitType";

                command.Parameters.Add(new SqlParameter("@VisitTypeName", SqlDbType.NVarChar, 50) { Value = visitType.VisitTypeName });
                command.Parameters.Add(new SqlParameter("@BaseRate", SqlDbType.Decimal) { Value = visitType.BaseRate });
                command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255) { Value = visitType.Description ?? (object)DBNull.Value });

                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT SCOPE_IDENTITY()";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding visit type: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(VisitType visitType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_UpdateVisitType";

                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = visitType.VisitTypeId });
                command.Parameters.Add(new SqlParameter("@VisitTypeName", SqlDbType.NVarChar, 50) { Value = visitType.VisitTypeName });
                command.Parameters.Add(new SqlParameter("@BaseRate", SqlDbType.Decimal) { Value = visitType.BaseRate });
                command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255) { Value = visitType.Description ?? (object)DBNull.Value });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating visit type: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int visitTypeId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_DeleteVisitType";

                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = visitTypeId });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting visit type: {ex.Message}", ex);
            }
        }
    }
}