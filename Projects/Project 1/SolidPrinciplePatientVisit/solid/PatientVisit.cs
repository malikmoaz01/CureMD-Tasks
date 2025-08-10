using System;
 
public enum UserRole
{
    Admin,
    Receptionist
}
 
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