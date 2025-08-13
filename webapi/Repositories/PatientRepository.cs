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

            // Add parameters with proper null handling
            command.Parameters.Add(new SqlParameter("@PatientName", SqlDbType.NVarChar, 100) 
            { 
                Value = string.IsNullOrWhiteSpace(patient.PatientName) ? (object)DBNull.Value : patient.PatientName.Trim() 
            });
            
            command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.Date) 
            { 
                Value = patient.DateOfBirth ?? (object)DBNull.Value 
            });
            
            command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 10) 
            { 
                Value = string.IsNullOrWhiteSpace(patient.Gender) ? (object)DBNull.Value : patient.Gender.Trim() 
            });
            
            command.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15) 
            { 
                Value = string.IsNullOrWhiteSpace(patient.ContactNumber) ? (object)DBNull.Value : patient.ContactNumber.Trim() 
            });
            
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) 
            { 
                Value = string.IsNullOrWhiteSpace(patient.Email) ? (object)DBNull.Value : patient.Email.Trim() 
            });
            
            command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 255) 
            { 
                Value = string.IsNullOrWhiteSpace(patient.Address) ? (object)DBNull.Value : patient.Address.Trim() 
            });
            
            command.Parameters.Add(new SqlParameter("@EmergencyContact", SqlDbType.NVarChar, 15) 
            { 
                Value = string.IsNullOrWhiteSpace(patient.EmergencyContact) ? (object)DBNull.Value : patient.EmergencyContact.Trim() 
            });

            // Execute and get the new patient ID
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            // Handle specific SQL Server errors
            if (ex.Number == 50008)
                throw new ArgumentException("Patient name is required.");
            if (ex.Number == 50009)
                throw new ArgumentException("Invalid gender. Must be Male, Female, or Other.");
                
            throw new InvalidOperationException($"Database error adding patient: {ex.Message}", ex);
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

        // Add parameters with proper null handling
        command.Parameters.Add(new SqlParameter("@PatientId", SqlDbType.Int) 
        { 
            Value = patient.PatientId 
        });
        
        command.Parameters.Add(new SqlParameter("@PatientName", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(patient.PatientName) ? (object)DBNull.Value : patient.PatientName.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.Date) 
        { 
            Value = patient.DateOfBirth ?? (object)DBNull.Value 
        });
        
        command.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 10) 
        { 
            Value = string.IsNullOrWhiteSpace(patient.Gender) ? (object)DBNull.Value : patient.Gender.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15) 
        { 
            Value = string.IsNullOrWhiteSpace(patient.ContactNumber) ? (object)DBNull.Value : patient.ContactNumber.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(patient.Email) ? (object)DBNull.Value : patient.Email.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 255) 
        { 
            Value = string.IsNullOrWhiteSpace(patient.Address) ? (object)DBNull.Value : patient.Address.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@EmergencyContact", SqlDbType.NVarChar, 15) 
        { 
            Value = string.IsNullOrWhiteSpace(patient.EmergencyContact) ? (object)DBNull.Value : patient.EmergencyContact.Trim() 
        });

        // Execute and get result
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
        // Handle specific SQL Server errors
        if (ex.Number == 50008)
            throw new ArgumentException("Patient name is required.");
        if (ex.Number == 50009)
            throw new ArgumentException("Invalid gender. Must be Male, Female, or Other.");
        if (ex.Number == 50010)
            throw new ArgumentException("Patient not found.");
            
        throw new InvalidOperationException($"Database error updating patient: {ex.Message}", ex);
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

        // Execute and get result
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
        // Handle specific SQL Server errors
        if (ex.Number == 50010)
            throw new ArgumentException("Patient not found.");
        if (ex.Number == 50012)
            throw new InvalidOperationException("Cannot delete patient with existing appointments.");
            
        throw new InvalidOperationException($"Database error deleting patient: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error deleting patient: {ex.Message}", ex);
    }
}
    }
}