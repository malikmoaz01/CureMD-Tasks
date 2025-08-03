using System.Collections.Generic;

// DIP: Dependency Inversion - High level modules depend on abstractions

// OCP & DIP: Repository abstraction for data operations
public interface IVisitRepository
{
    void SaveVisits(List<PatientVisit> visits);
    List<PatientVisit> LoadVisits();
}

// OCP & DIP: Logger abstraction
public interface ILogger
{
    void LogActivity(string action, bool success, string details = "");
}

// OCP & DIP: Notification service abstraction
public interface INotificationService
{
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
    bool GetConfirmation(string message);
}

// OCP & DIP: Fee calculation abstraction
public interface IFeeCalculator
{
    decimal CalculateFee(string visitType, int duration);
    void LoadFeeRates();
    void SaveFeeRates();
}

// OCP & DIP: Display service abstraction
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

// LSP: Command pattern interface
public interface IPatientCommand
{
    void Execute();
    void Undo();
}