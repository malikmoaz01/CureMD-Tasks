using System.Data;
using Microsoft.Data.SqlClient;
using webapi.Models;
using webapi.Data;

namespace webapi.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor> GetByIdAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<int> AddAsync(Doctor doctor);
        Task<bool> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(int doctorId);
    }

    public class DoctorRepository : IDoctorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DoctorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Doctor> GetByIdAsync(int doctorId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetDoctorById";

                command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) { Value = doctorId });

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Doctor
                    {
                        DoctorId = reader.GetInt32("DoctorId"),
                        DoctorName = reader.GetString("DoctorName"),
                        Specialization = reader.IsDBNull("Specialization") ? null : reader.GetString("Specialization"),
                        ContactNumber = reader.IsDBNull("ContactNumber") ? null : reader.GetString("ContactNumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving doctor: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            try
            {
                var doctors = new List<Doctor>();

                using var connection = _connectionFactory.CreateConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "stp_GetAllDoctors";

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    doctors.Add(new Doctor
                    {
                        DoctorId = reader.GetInt32("DoctorId"),
                        DoctorName = reader.GetString("DoctorName"),
                        Specialization = reader.IsDBNull("Specialization") ? null : reader.GetString("Specialization"),
                        ContactNumber = reader.IsDBNull("ContactNumber") ? null : reader.GetString("ContactNumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        CreatedDate = reader.GetDateTime("CreatedDate")
                    });
                }

                return doctors;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving all doctors: {ex.Message}", ex);
            }
        }

public async Task<int> AddAsync(Doctor doctor)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_AddDoctor";
 
        command.Parameters.Add(new SqlParameter("@DoctorName", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.DoctorName) ? (object)DBNull.Value : doctor.DoctorName.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@Specialization", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.Specialization) ? (object)DBNull.Value : doctor.Specialization.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.ContactNumber) ? (object)DBNull.Value : doctor.ContactNumber.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.Email) ? (object)DBNull.Value : doctor.Email.Trim() 
        });
 
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    catch (SqlException ex)
    { 
        if (ex.Number == 50006)
            throw new ArgumentException("Doctor name is required.");
        if (ex.Number == 50007)
            throw new ArgumentException("Doctor not found.");
            
        throw new InvalidOperationException($"Database error adding doctor: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error adding doctor: {ex.Message}", ex);
    }
}

public async Task<bool> DeleteAsync(int doctorId)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_DeleteDoctor";

        command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) { Value = doctorId });
 
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
        if (ex.Number == 50007)
            throw new ArgumentException("Doctor not found.");
        if (ex.Number == 50011)
            throw new InvalidOperationException("Cannot delete doctor with existing appointments.");
            
        throw new InvalidOperationException($"Cant Delete Due to Refrential Integrity: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error deleting doctor: {ex.Message}", ex);
    }
}

public async Task<bool> UpdateAsync(Doctor doctor)
{
    try
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "stp_UpdateDoctor";
 
        command.Parameters.Add(new SqlParameter("@DoctorId", SqlDbType.Int) 
        { 
            Value = doctor.DoctorId 
        });
        
        command.Parameters.Add(new SqlParameter("@DoctorName", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.DoctorName) ? (object)DBNull.Value : doctor.DoctorName.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@Specialization", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.Specialization) ? (object)DBNull.Value : doctor.Specialization.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@ContactNumber", SqlDbType.NVarChar, 15) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.ContactNumber) ? (object)DBNull.Value : doctor.ContactNumber.Trim() 
        });
        
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) 
        { 
            Value = string.IsNullOrWhiteSpace(doctor.Email) ? (object)DBNull.Value : doctor.Email.Trim() 
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
        if (ex.Number == 50006)
            throw new ArgumentException("Doctor name is required.");
        if (ex.Number == 50007)
            throw new ArgumentException("Doctor not found.");
            
        throw new InvalidOperationException($"Database error updating doctor: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error updating doctor: {ex.Message}", ex);
    }
}
    }
}
