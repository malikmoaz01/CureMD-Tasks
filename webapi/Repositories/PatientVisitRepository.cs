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
        private readonly IActivityLogRepository _activityLogRepository;

        public PatientVisitRepository(IDbConnectionFactory connectionFactory, IActivityLogRepository activityLogRepository)
        {
            _connectionFactory = connectionFactory;
            _activityLogRepository = activityLogRepository;
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
                    var patientVisit = new PatientVisit
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

                    await LogActivity("View Patient Visit", true, $"Viewed patient visit ID: {id}", patientVisit.CreatedBy, id);
                    return patientVisit;
                }

                return null;
            }
            catch (Exception ex)
            {
                await LogActivity("View Patient Visit", false, $"Failed to view patient visit ID: {id}. Error: {ex.Message}", null, id);
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

                await LogActivity("View All Patient Visits", true, $"Retrieved {patientVisits.Count} patient visits", null, null);
                return patientVisits;
            }
            catch (Exception ex)
            {
                await LogActivity("View All Patient Visits", false, $"Failed to retrieve patient visits. Error: {ex.Message}", null, null);
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

                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) 
                { 
                    Value = patientVisit.PatientId 
                });
                
                command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) 
                { 
                    Value = patientVisit.DoctorId ?? (object)DBNull.Value 
                });
                
                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) 
                { 
                    Value = patientVisit.VisitTypeId 
                });
                
                command.Parameters.Add(new SqlParameter("@VisitDate", SqlDbType.DateTime2) 
                { 
                    Value = patientVisit.VisitDate 
                });
                
                command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar, 500) 
                { 
                    Value = string.IsNullOrWhiteSpace(patientVisit.Note) ? (object)DBNull.Value : patientVisit.Note.Trim() 
                });
                
                command.Parameters.Add(new SqlParameter("@DurationInMinutes", SqlDbType.Int) 
                { 
                    Value = patientVisit.DurationInMinutes 
                });
                
                command.Parameters.Add(new SqlParameter("@Fee", SqlDbType.Decimal) 
                { 
                    Value = patientVisit.Fee 
                });
                
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) 
                { 
                    Value = patientVisit.CreatedBy ?? (object)DBNull.Value 
                });

                var result = await command.ExecuteScalarAsync();
                int newVisitId = Convert.ToInt32(result);

                await LogActivity("Add Patient Visit", true, 
                    $"Added new patient visit for Patient ID: {patientVisit.PatientId}, Visit Date: {patientVisit.VisitDate:yyyy-MM-dd}", 
                    patientVisit.CreatedBy, newVisitId);

                return newVisitId;
            }
            catch (SqlException ex)
            {
                await LogActivity("Add Patient Visit", false, 
                    $"Failed to add patient visit for Patient ID: {patientVisit.PatientId}. Error: {ex.Message}", 
                    patientVisit.CreatedBy, null);

                if (ex.Number == 50013)
                    throw new ArgumentException("Valid PatientId is required.");
                if (ex.Number == 50014)
                    throw new ArgumentException("Valid VisitTypeId is required.");
                if (ex.Number == 50015)
                    throw new ArgumentException("VisitDate is required.");
                if (ex.Number == 50016)
                    throw new ArgumentException("Valid DurationInMinutes is required.");
                if (ex.Number == 50017)
                    throw new ArgumentException("Valid Fee is required.");
                if (ex.Number == 50018)
                    throw new ArgumentException("Patient not found.");
                if (ex.Number == 50019)
                    throw new ArgumentException("Doctor not found.");
                    
                throw new InvalidOperationException($"Database error adding patient visit: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await LogActivity("Add Patient Visit", false, 
                    $"Failed to add patient visit for Patient ID: {patientVisit.PatientId}. Error: {ex.Message}", 
                    patientVisit.CreatedBy, null);
                    
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

                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) 
                { 
                    Value = patientVisit.Id 
                });
                
                command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) 
                { 
                    Value = patientVisit.PatientId 
                });
                
                command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) 
                { 
                    Value = patientVisit.DoctorId ?? (object)DBNull.Value 
                });
                
                command.Parameters.Add(new SqlParameter("@VisitTypeId", SqlDbType.Int) 
                { 
                    Value = patientVisit.VisitTypeId 
                });
                
                command.Parameters.Add(new SqlParameter("@VisitDate", SqlDbType.DateTime2) 
                { 
                    Value = patientVisit.VisitDate 
                });
                
                command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar, 500) 
                { 
                    Value = string.IsNullOrWhiteSpace(patientVisit.Note) ? (object)DBNull.Value : patientVisit.Note.Trim() 
                });
                
                command.Parameters.Add(new SqlParameter("@DurationInMinutes", SqlDbType.Int) 
                { 
                    Value = patientVisit.DurationInMinutes 
                });
                
                command.Parameters.Add(new SqlParameter("@Fee", SqlDbType.Decimal) 
                { 
                    Value = patientVisit.Fee 
                });
                
                command.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.Int) 
                { 
                    Value = patientVisit.ModifiedBy ?? (object)DBNull.Value 
                });

                var result = await command.ExecuteScalarAsync();
                
                if (result != null && result != DBNull.Value)
                {
                    int rowsAffected = Convert.ToInt32(result);
                    bool isSuccess = rowsAffected > 0;

                    await LogActivity("Update Patient Visit", isSuccess, 
                        isSuccess ? $"Updated patient visit ID: {patientVisit.Id}" : $"Failed to update patient visit ID: {patientVisit.Id} - No rows affected", 
                        patientVisit.ModifiedBy, patientVisit.Id);

                    return isSuccess;
                }
                
                await LogActivity("Update Patient Visit", false, $"Failed to update patient visit ID: {patientVisit.Id} - No result returned", 
                    patientVisit.ModifiedBy, patientVisit.Id);
                return false;
            }
            catch (SqlException ex)
            {
                await LogActivity("Update Patient Visit", false, 
                    $"Failed to update patient visit ID: {patientVisit.Id}. Error: {ex.Message}", 
                    patientVisit.ModifiedBy, patientVisit.Id);

                if (ex.Number == 50013)
                    throw new ArgumentException("Valid PatientId is required.");
                if (ex.Number == 50014)
                    throw new ArgumentException("Valid VisitTypeId is required.");
                if (ex.Number == 50015)
                    throw new ArgumentException("VisitDate is required.");
                if (ex.Number == 50016)
                    throw new ArgumentException("Valid DurationInMinutes is required.");
                if (ex.Number == 50017)
                    throw new ArgumentException("Valid Fee is required.");
                if (ex.Number == 50018)
                    throw new ArgumentException("Patient not found.");
                if (ex.Number == 50019)
                    throw new ArgumentException("Doctor not found.");
                if (ex.Number == 50020)
                    throw new ArgumentException("Patient visit not found.");
                    
                throw new InvalidOperationException($"Database error updating patient visit: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await LogActivity("Update Patient Visit", false, 
                    $"Failed to update patient visit ID: {patientVisit.Id}. Error: {ex.Message}", 
                    patientVisit.ModifiedBy, patientVisit.Id);
                    
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

                var result = await command.ExecuteScalarAsync();
                
                if (result != null && result != DBNull.Value)
                {
                    int rowsAffected = Convert.ToInt32(result);
                    bool isSuccess = rowsAffected > 0;

                    await LogActivity("Delete Patient Visit", isSuccess, 
                        isSuccess ? $"Deleted patient visit ID: {id}" : $"Failed to delete patient visit ID: {id} - No rows affected", 
                        null, id);

                    return isSuccess;
                }
                
                await LogActivity("Delete Patient Visit", false, $"Failed to delete patient visit ID: {id} - No result returned", 
                    null, id);
                return false;
            }
            catch (SqlException ex)
            {
                await LogActivity("Delete Patient Visit", false, 
                    $"Failed to delete patient visit ID: {id}. Error: {ex.Message}", 
                    null, id);

                if (ex.Number == 50020)
                    throw new ArgumentException("Patient visit not found.");
                    
                throw new InvalidOperationException($"Database error deleting patient visit: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                await LogActivity("Delete Patient Visit", false, 
                    $"Failed to delete patient visit ID: {id}. Error: {ex.Message}", 
                    null, id);
                    
                throw new InvalidOperationException($"Error deleting patient visit: {ex.Message}", ex);
            }
        }

        private async Task LogActivity(string action, bool success, string details, int? userId, int? visitId)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    Action = action,
                    Success = success,
                    Details = details,
                    UserId = userId,
                    VisitId = visitId
                };

                await _activityLogRepository.AddAsync(activityLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log activity: {ex.Message}");
            }
        }
    }
}