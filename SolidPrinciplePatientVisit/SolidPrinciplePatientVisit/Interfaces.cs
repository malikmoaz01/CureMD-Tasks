using System.Collections.Generic;
 
public interface IVisitRepository
{
    void SaveVisits(List<PatientVisit> visits);
    List<PatientVisit> LoadVisits();
}
  
public interface ILogger
{
    void LogActivity(string action, bool success, string details = "");
}
 
public interface INotificationService
{
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
    bool GetConfirmation(string message);
}
 
public interface IFeeCalculator
{
    decimal CalculateFee(string visitType, int duration);
    void LoadFeeRates();
    void SaveFeeRates();
}
 
public interface IDisplayService
{
    void ShowVisit(PatientVisit visit);
    void ShowAllRecords(List<PatientVisit> visits);
    void ShowFilteredAndSortedRecords(List<PatientVisit> visitList);
    void ShowVisitsByType(List<PatientVisit> visits);
    void GenerateVisitSummary(PatientVisit visit);
    void GetTotalVisitsByType(List<PatientVisit> visits);
    void GetWeeklySummary(List<PatientVisit> visits);
}
 
public interface IPatientCommand
{
    void Execute();
    void Undo();
}