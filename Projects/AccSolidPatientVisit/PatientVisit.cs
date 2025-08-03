using System;

// SRP: Single Responsibility - Data Model Only
public enum UserRole
{
    Admin,
    Receptionist
}

// SRP: Single Responsibility - Patient Visit Data Structure
public class PatientVisit
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitType { get; set; }
    public string Note { get; set; }
    public string DoctorName { get; set; }
    public int DurationInMinutes { get; set; }
    public decimal Fee { get; set; }
}