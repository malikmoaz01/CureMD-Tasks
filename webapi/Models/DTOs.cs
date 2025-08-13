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
    [Required(ErrorMessage = "Doctor name is required")]
    [StringLength(100, ErrorMessage = "Doctor name cannot exceed 100 characters")]
    public string DoctorName { get; set; }

    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
    public string? Specialization { get; set; }

    [StringLength(15, ErrorMessage = "Contact number cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactNumber { get; set; }

    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
}

public class UpdateDoctorDto
{
    [Required]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Doctor name is required")]
    [StringLength(100, ErrorMessage = "Doctor name cannot exceed 100 characters")]
    public string DoctorName { get; set; }

    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
    public string? Specialization { get; set; }

    [StringLength(15, ErrorMessage = "Contact number cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactNumber { get; set; }

    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
}


    public class CreatePatientDto
{
    [Required(ErrorMessage = "Patient name is required")]
    [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
    public string PatientName { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
    public string? Gender { get; set; }

    [StringLength(15, ErrorMessage = "Contact number cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactNumber { get; set; }

    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    public string? Address { get; set; }

    [StringLength(15, ErrorMessage = "Emergency contact cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Invalid emergency contact format")]
    public string? EmergencyContact { get; set; }
}

public class UpdatePatientDto
{
    [Required]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Patient name is required")]
    [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
    public string PatientName { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
    public string? Gender { get; set; }

    [StringLength(15, ErrorMessage = "Contact number cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactNumber { get; set; }

    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    public string? Address { get; set; }

    [StringLength(15, ErrorMessage = "Emergency contact cannot exceed 15 characters")]
    [Phone(ErrorMessage = "Invalid emergency contact format")]
    public string? EmergencyContact { get; set; }
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
    [Required(ErrorMessage = "Patient ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be greater than 0")]
    public int PatientId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be greater than 0")]
    public int? DoctorId { get; set; }

    [Required(ErrorMessage = "Visit Type ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Visit Type ID must be greater than 0")]
    public int VisitTypeId { get; set; }

    [Required(ErrorMessage = "Visit Date is required")]
    public DateTime VisitDate { get; set; }

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string? Note { get; set; }

    [Required(ErrorMessage = "Duration in minutes is required")]
    [Range(1, 480, ErrorMessage = "Duration must be between 1 and 480 minutes")]
    public int DurationInMinutes { get; set; }

    [Required(ErrorMessage = "Fee is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Fee must be greater than or equal to 0")]
    public decimal Fee { get; set; }
}

public class UpdatePatientVisitDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Patient ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be greater than 0")]
    public int PatientId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be greater than 0")]
    public int? DoctorId { get; set; }

    [Required(ErrorMessage = "Visit Type ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Visit Type ID must be greater than 0")]
    public int VisitTypeId { get; set; }

    [Required(ErrorMessage = "Visit Date is required")]
    public DateTime VisitDate { get; set; }

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string? Note { get; set; }

    [Required(ErrorMessage = "Duration in minutes is required")]
    [Range(1, 480, ErrorMessage = "Duration must be between 1 and 480 minutes")]
    public int DurationInMinutes { get; set; }

    [Required(ErrorMessage = "Fee is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Fee must be greater than or equal to 0")]
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