using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IPatientVisitRepository
    {
        Task<PatientVisit> GetByIdAsync(int id);
        Task<IEnumerable<PatientVisit>> GetAllAsync();
        Task<int> AddAsync(PatientVisit patientVisit);
        Task<bool> UpdateAsync(PatientVisit patientVisit);
        Task<bool> DeleteAsync(int id);
    }

    public class PatientVisitRepository : IPatientVisitRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PatientVisitRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PatientVisit> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetPatientVisitById";

                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new PatientVisit
                    {
                        Id = reader.GetInt32("Id"),
                        PatientId = reader.GetInt32("PatientId"),
                        DoctorId = reader.IsDBNull("DoctorId") ? null : reader.GetInt32("DoctorId"),
                        VisitTypeId = reader.GetInt32("VisitTypeId"),
                        VisitDate = reader.GetDateTime("VisitDate"),
                        Note = reader.IsDBNull("Note") ? null : reader.GetString("Note"),
                        DurationInMinutes = reader.GetInt32("DurationInMinutes"),
                        Fee = reader.GetDecimal("Fee"),
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetInt32("CreatedBy"),
                        ModifiedDate = reader.IsDBNull("ModifiedDate") ? null : reader.GetDateTime("ModifiedDate"),
                        ModifiedBy = reader.IsDBNull("ModifiedBy") ? null : reader.GetInt32("ModifiedBy")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving patient visit: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PatientVisit>> GetAllAsync()
        {
            try
            {
                var patientVisits = new List<PatientVisit>();

                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllPatientVisits";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    patientVisits.Add(new PatientVisit
                    {
                        Id = reader.GetInt32("Id"),
                        PatientId = reader.GetInt32("PatientId"),
                        DoctorId = reader.IsDBNull("DoctorId") ? null : reader.GetInt32("DoctorId"),
                        VisitTypeId = reader.GetInt32("VisitTypeId"),
                        VisitDate = reader.GetDateTime("VisitDate"),
                        Note = reader.IsDBNull("Note") ? null : reader.GetString("Note"),
                        DurationInMinutes = reader.GetInt32("DurationInMinutes"),
                        Fee = reader.GetDecimal("Fee"),
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetInt32("CreatedBy"),
                        ModifiedDate = reader.IsDBNull("ModifiedDate") ? null : reader.GetDateTime("ModifiedDate"),
                        ModifiedBy = reader.IsDBNull("ModifiedBy") ? null : reader.GetInt32("ModifiedBy")
                    });
                }

                return patientVisits;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all patient visits: {ex.Message}", ex);
            }
        }

        public async Task<int> AddAsync(PatientVisit patientVisit)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_AddPatientVisit";

                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) { Value = patientVisit.PatientId });
                command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) { Value = patientVisit.DoctorId ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = patientVisit.VisitTypeId });
                command.Parameters.Add(new SqlParameter("@VisitDate", SqlDbType.DateTime2) { Value = patientVisit.VisitDate });
                command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar, 500) { Value = patientVisit.Note ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DurationInMinutes", SqlDbType.Int) { Value = patientVisit.DurationInMinutes });
                command.Parameters.Add(new SqlParameter("@Fee", SqlDbType.Decimal) { Value = patientVisit.Fee });
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = patientVisit.CreatedBy ?? (object)DBNull.Value });

                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT SCOPE_IDENTITY()";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding patient visit: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(PatientVisit patientVisit)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_UpdatePatientVisit";

                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = patientVisit.Id });
                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) { Value = patientVisit.PatientId });
                command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) { Value = patientVisit.DoctorId ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) { Value = patientVisit.VisitTypeId });
                command.Parameters.Add(new SqlParameter("@VisitDate", SqlDbType.DateTime2) { Value = patientVisit.VisitDate });
                command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar, 500) { Value = patientVisit.Note ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@DurationInMinutes", SqlDbType.Int) { Value = patientVisit.DurationInMinutes });
                command.Parameters.Add(new SqlParameter("@Fee", SqlDbType.Decimal) { Value = patientVisit.Fee });
                command.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.Int) { Value = patientVisit.ModifiedBy ?? (object)DBNull.Value });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating patient visit: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_DeletePatientVisit";

                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting patient visit: {ex.Message}", ex);
            }
        }
    }
}