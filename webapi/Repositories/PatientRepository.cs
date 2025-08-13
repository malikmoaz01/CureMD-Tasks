using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient> GetByIdAsync(int patientId);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<int> AddAsync(Patient patient);
        Task<bool> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int patientId);
    }

    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PatientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Patient> GetByIdAsync(int patientId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetPatientById";

                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) { Value = patientId });

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        PatientName = reader.GetString("PatientName"),
                        DateOfBirth = reader.IsDBNull("DateOfBirth") ? null : reader.GetDateTime("DateOfBirth"),
                        Gender = reader.IsDBNull("Gender") ? null : reader.GetString("Gender"),
                        ContactNumber = reader.IsDBNull("ContactNumber") ? null : reader.GetString("ContactNumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                        EmergencyContact = reader.IsDBNull("EmergencyContact") ? null : reader.GetString("EmergencyContact"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving patient: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            try
            {
                var patients = new List<Patient>();

                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllPatients";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    patients.Add(new Patient
                    {
                        PatientId = reader.GetInt32("PatientId"),
                        PatientName = reader.GetString("PatientName"),
                        DateOfBirth = reader.IsDBNull("DateOfBirth") ? null : reader.GetDateTime("DateOfBirth"),
                        Gender = reader.IsDBNull("Gender") ? null : reader.GetString("Gender"),
                        ContactNumber = reader.IsDBNull("ContactNumber") ? null : reader.GetString("ContactNumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Address = reader.IsDBNull("Address") ? null : reader.GetString("Address"),
                        EmergencyContact = reader.IsDBNull("EmergencyContact") ? null : reader.GetString("EmergencyContact"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    });
                }

                return patients;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all patients: {ex.Message}", ex);
            }
        }

        public async Task<int> AddAsync(Patient patient)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_AddPatient";

                command.Parameters.Add(new SqlParameter("@PatientName", SqlDbType.NVarChar, 100) { Value = patient.PatientName });
                command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = patient.DateOfBirth ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 10) { Value = patient.Gender ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15) { Value = patient.ContactNumber ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = patient.Email ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 255) { Value = patient.Address ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@EmergencyContact", SqlDbType.NVarChar, 15) { Value = patient.EmergencyContact ?? (object)DBNull.Value });

                await command.ExecuteNonQueryAsync();

                command.CommandText = "SELECT SCOPE_IDENTITY()";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding patient: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_UpdatePatient";

                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) { Value = patient.PatientId });
                command.Parameters.Add(new SqlParameter("@PatientName", SqlDbType.NVarChar, 100) { Value = patient.PatientName });
                command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = patient.DateOfBirth ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 10) { Value = patient.Gender ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15) { Value = patient.ContactNumber ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = patient.Email ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 255) { Value = patient.Address ?? (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@EmergencyContact", SqlDbType.NVarChar, 15) { Value = patient.EmergencyContact ?? (object)DBNull.Value });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error updating patient: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int patientId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_DeletePatient";

                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) { Value = patientId });

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting patient: {ex.Message}", ex);
            }
        }
    }
}