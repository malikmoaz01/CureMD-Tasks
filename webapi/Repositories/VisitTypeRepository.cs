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
                        VisitTypeId = reader.GetInt32(reader.GetOrdinal("VisitTypeId")),
                        VisitTypeName = reader.GetString(reader.GetOrdinal("VisitTypeName")),
                        BaseRate = reader.GetDecimal(reader.GetOrdinal("BaseRate")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Description"))
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
                        VisitTypeId = reader.GetInt32(reader.GetOrdinal("VisitTypeId")),
                        VisitTypeName = reader.GetString(reader.GetOrdinal("VisitTypeName")),
                        BaseRate = reader.GetDecimal(reader.GetOrdinal("BaseRate")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Description"))
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

                command.Parameters.Add(new SqlParameter("@VisitTypeName", SqlDbType.NVarChar, 50)
                {
                    Value = string.IsNullOrWhiteSpace(visitType.VisitTypeName) ? (object)DBNull.Value : visitType.VisitTypeName.Trim()
                });

                command.Parameters.Add(new SqlParameter("@BaseRate", SqlDbType.Decimal)
                {
                    Precision = 10,
                    Scale = 2,
                    Value = visitType.BaseRate
                });

                command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(visitType.Description) ? (object)DBNull.Value : visitType.Description.Trim()
                });

                var result = await command.ExecuteScalarAsync();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Failed to get new visit type ID");

                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"SQL Exception: Number={ex.Number}, Message={ex.Message}");

                if (ex.Number == 50011)
                    throw new ArgumentException("Visit type name is required.");
                if (ex.Number == 50013)
                    throw new ArgumentException("Base rate must be a positive value.");
                if (ex.Number == 50014)
                    throw new ArgumentException("Visit type with this name already exists.");

                throw new InvalidOperationException($"Database error adding visit type: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"General Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
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

                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int)
                {
                    Value = visitType.VisitTypeId
                });

                command.Parameters.Add(new SqlParameter("@VisitTypeName", SqlDbType.NVarChar, 50)
                {
                    Value = string.IsNullOrWhiteSpace(visitType.VisitTypeName) ? (object)DBNull.Value : visitType.VisitTypeName.Trim()
                });

                command.Parameters.Add(new SqlParameter("@BaseRate", SqlDbType.Decimal)
                {
                    Value = visitType.BaseRate
                });

                command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(visitType.Description) ? (object)DBNull.Value : visitType.Description.Trim()
                });

                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    int rowsAffected = Convert.ToInt32(result);
                    return rowsAffected > 0;
                }

                return false;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50011)
                    throw new ArgumentException("Visit type name is required.");
                if (ex.Number == 50012)
                    throw new ArgumentException("Visit type not found.");
                if (ex.Number == 50013)
                    throw new ArgumentException("Base rate must be a positive value.");
                if (ex.Number == 50014)
                    throw new ArgumentException("Visit type with this name already exists.");

                throw new InvalidOperationException($"Database error updating visit type: {ex.Message}", ex);
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

                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    int rowsAffected = Convert.ToInt32(result);
                    return rowsAffected > 0;
                }

                return false;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50012)
                    throw new ArgumentException("Visit type not found.");

                throw new InvalidOperationException($"Database error deleting visit type: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting visit type: {ex.Message}", ex);
            }
        }
    }
}