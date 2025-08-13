using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Password { get; set; }
        
        [Required]
        public string UserRole { get; set; }
    }

    public class CreateDoctorDto
    {
        [Required]
        [StringLength(100)]
        public string DoctorName { get; set; }
        
        [StringLength(100)]
        public string Specialization { get; set; }
        
        [StringLength(15)]
        public string ContactNumber { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
    }

    public class UpdateDoctorDto
    {
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string DoctorName { get; set; }
        
        [StringLength(100)]
        public string Specialization { get; set; }
        
        [StringLength(15)]
        public string ContactNumber { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
    }

    public class CreatePatientDto
    {
        [Required]
        [StringLength(100)]
        public string PatientName { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string Gender { get; set; }
        
        [StringLength(15)]
        public string ContactNumber { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(255)]
        public string Address { get; set; }
        
        [StringLength(15)]
        public string EmergencyContact { get; set; }
    }

    public class UpdatePatientDto
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PatientName { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string Gender { get; set; }
        
        [StringLength(15)]
        public string ContactNumber { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(255)]
        public string Address { get; set; }
        
        [StringLength(15)]
        public string EmergencyContact { get; set; }
    }

    public class CreateVisitTypeDto
    {
        [Required]
        [StringLength(50)]
        public string VisitTypeName { get; set; }
        
        public decimal BaseRate { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
    }

    public class CreatePatientVisitDto
    {
        [Required]
        public int PatientId { get; set; }
        
        public int? DoctorId { get; set; }
        
        [Required]
        public int VisitTypeId { get; set; }
        
        [Required]
        public DateTime VisitDate { get; set; }
        
        [StringLength(500)]
        public string Note { get; set; }
        
        public int DurationInMinutes { get; set; } = 30;
        
        [Required]
        public decimal Fee { get; set; }
    }

    public class UpdatePatientVisitDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public int PatientId { get; set; }
        
        public int? DoctorId { get; set; }
        
        [Required]
        public int VisitTypeId { get; set; }
        
        [Required]
        public DateTime VisitDate { get; set; }
        
        [StringLength(500)]
        public string Note { get; set; }
        
        public int DurationInMinutes { get; set; }
        
        [Required]
        public decimal Fee { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string UserRole { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        
        public static ApiResponse<T> SuccessResult(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message };
        }
        
        public static ApiResponse<T> ErrorResult(string message)
        {
            return new ApiResponse<T> { Success = false, Message = message };
        }
    }
}