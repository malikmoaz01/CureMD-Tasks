using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Doctor
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Patient
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VisitType
    {
        public int VisitTypeId { get; set; }
        public string VisitTypeName { get; set; }
        public decimal BaseRate { get; set; }
        public string Description { get; set; }
    }

    public class PatientVisit
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int VisitTypeId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Note { get; set; }
        public int DurationInMinutes { get; set; }
        public decimal Fee { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class FeeRate
    {
        public int FeeRateId { get; set; }
        public int VisitTypeId { get; set; }
        public decimal BaseRate { get; set; }
        public decimal ExtraTimeRate { get; set; }
        public int ExtraTimeThreshold { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    public class ActivityLog
    {
        public int LogId { get; set; }
        public DateTime LogDateTime { get; set; }
        public string Action { get; set; }
        public bool Success { get; set; }
        public string Details { get; set; }
        public int? UserId { get; set; }
        public int? VisitId { get; set; }
    }
}