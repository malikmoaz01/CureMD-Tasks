using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IFeeRateRepository
    {
        Task<FeeRate> GetByIdAsync(int feeRateId);
        Task<IEnumerable<FeeRate>> GetAllAsync();
        Task<int> AddAsync(FeeRate feeRate);
        Task<bool> UpdateAsync(FeeRate feeRate);
        Task<bool> DeleteAsync(int feeRateId);
    }

    public class FeeRateRepository : IFeeRateRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FeeRateRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<FeeRate> GetByIdAsync(int feeRateId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetFeeRateById";

                command.Parameters.Add(new SqlParameter("@FeeRateId", SqlDbType.Int) { Value = feeRateId });

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new FeeRate
                    {
                        FeeRateId = reader.GetInt32("FeeRateId"),
                        VisitTypeId = reader.GetInt32("VisitTypeId"),
                        BaseRate = reader.GetDecimal("BaseRate"),
                        ExtraTimeRate = reader.GetDecimal("ExtraTimeRate"),
                        ExtraTimeThreshold = reader.GetInt32("ExtraTimeThreshold"),
                        EffectiveDate = reader.GetDateTime("EffectiveDate")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving fee rate: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<FeeRate>> GetAllAsync()
        {
            try
            {
                var feeRates = new List<FeeRate>();

                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllFeeRates";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    feeRates.Add(new FeeRate
                    {
                        FeeRateId = reader.GetInt32("FeeRateId"),
                        VisitTypeId = reader.GetInt32("VisitTypeId"),
                        BaseRate = reader.GetDecimal("BaseRate"),
                        ExtraTimeRate = reader.GetDecimal("ExtraTimeRate"),
                        ExtraTimeThreshold = reader.GetInt32("ExtraTimeThreshold"),
                        EffectiveDate = reader.GetDateTime("EffectiveDate")
                    });
                }

                return feeRates;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all fee rates: {ex.Message}", ex);
            }
        }

        public async Task<int> AddAsync(FeeRate feeRate)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_AddFeeRate";

        command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = feeRate.VisitTypeId });
        command.Parameters.Add(new SqlParameter("@BaseRate", SqlDbType.Decimal) { Value = feeRate.BaseRate });
        command.Parameters.Add(new SqlParameter("@ExtraTimeRate", SqlDbType.Decimal) { Value = feeRate.ExtraTimeRate });
        command.Parameters.Add(new SqlParameter("@ExtraTimeThreshold", SqlDbType.Int) { Value = feeRate.ExtraTimeThreshold });

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    catch (SqlException sqlEx)
    {
        Console.WriteLine($"SQL Error in AddAsync: {sqlEx.Number} - {sqlEx.Message}");
        throw sqlEx.Number switch
        {
            50021 => new ArgumentException("Valid VisitTypeId is required"),
            50022 => new ArgumentException("Valid BaseRate is required"),
            50023 => new ArgumentException("Valid ExtraTimeRate is required"),
            50024 => new ArgumentException("Valid ExtraTimeThreshold is required"),
            50025 => new ArgumentException("VisitType not found"),
            _ => new InvalidOperationException($"Error adding fee rate: {sqlEx.Message}", sqlEx)
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General Error in AddAsync: {ex.Message}");
        throw new InvalidOperationException($"Error adding fee rate: {ex.Message}", ex);
    }
}

public async Task<bool> UpdateAsync(FeeRate feeRate)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_UpdateFeeRate";

        command.Parameters.Add(new SqlParameter("@FeeRateId", SqlDbType.Int) { Value = feeRate.FeeRateId });
        command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = feeRate.VisitTypeId });
        command.Parameters.Add(new SqlParameter("@BaseRate", SqlDbType.Decimal) { Value = feeRate.BaseRate });
        command.Parameters.Add(new SqlParameter("@ExtraTimeRate", SqlDbType.Decimal) { Value = feeRate.ExtraTimeRate });
        command.Parameters.Add(new SqlParameter("@ExtraTimeThreshold", SqlDbType.Int) { Value = feeRate.ExtraTimeThreshold });

        var result = await command.ExecuteScalarAsync();
        var rowsAffected = Convert.ToInt32(result);
        
        Console.WriteLine($"UpdateAsync - Rows affected: {rowsAffected}");
        return rowsAffected > 0;
    }
    catch (SqlException sqlEx)
    {
        Console.WriteLine($"SQL Error in UpdateAsync: {sqlEx.Number} - {sqlEx.Message}");
        throw sqlEx.Number switch
        {
            50021 => new ArgumentException("Valid VisitTypeId is required"),
            50022 => new ArgumentException("Valid BaseRate is required"),
            50023 => new ArgumentException("Valid ExtraTimeRate is required"),
            50024 => new ArgumentException("Valid ExtraTimeThreshold is required"),
            50025 => new ArgumentException("VisitType not found"),
            50026 => new ArgumentException("Valid FeeRateId is required"),
            50027 => new ArgumentException("FeeRate not found"),
            _ => new InvalidOperationException($"Error updating fee rate: {sqlEx.Message}", sqlEx)
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General Error in UpdateAsync: {ex.Message}");
        throw new InvalidOperationException($"Error updating fee rate: {ex.Message}", ex);
    }
}

public async Task<bool> DeleteAsync(int feeRateId)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_DeleteFeeRate";

        command.Parameters.Add(new SqlParameter("@FeeRateId", SqlDbType.Int) { Value = feeRateId });

        var result = await command.ExecuteScalarAsync();
        var rowsAffected = Convert.ToInt32(result);
        
        Console.WriteLine($"DeleteAsync - Rows affected: {rowsAffected}");
        return rowsAffected > 0;
    }
    catch (SqlException sqlEx)
    {
        Console.WriteLine($"SQL Error in DeleteAsync: {sqlEx.Number} - {sqlEx.Message}");
        throw sqlEx.Number switch
        {
            50026 => new ArgumentException("Valid FeeRateId is required"),
            50027 => new ArgumentException("FeeRate not found"),
            _ => new InvalidOperationException($"Error deleting fee rate: {sqlEx.Message}", sqlEx)
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General Error in DeleteAsync: {ex.Message}");
        throw new InvalidOperationException($"Error deleting fee rate: {ex.Message}", ex);
    }
}
    }
}