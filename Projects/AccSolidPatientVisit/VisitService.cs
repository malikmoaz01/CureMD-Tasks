using System;
using System.Collections.Generic;
using System.Linq;

// SRP: Single Responsibility - High-level visit operations orchestration
// OCP: Open for extension through dependency injection
// DIP: Depends on abstractions, not concretions
public class VisitService
{
    private readonly IVisitManager _visitManager;
    private readonly CommandManager _commandManager;
    private readonly IDisplayService _displayService;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public VisitService(
        IVisitManager visitManager,
        CommandManager commandManager,
        IDisplayService displayService,
        INotificationService notificationService,
        ILogger logger)
    {
        _visitManager = visitManager;
        _commandManager = commandManager;
        _displayService = displayService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public void AddVisit(string patientName, DateTime visitDate, string visitType, string note, string doctorName = "", int duration = 30)
    {
        if (CheckTimeSlotConflict(patientName, visitDate))
        {
            bool proceed = _notificationService.GetConfirmation("Warning: This patient has another visit within 30 minutes. Proceed? (Y/N)");
            if (!proceed)
            {
                _logger.LogActivity("Add Visit", false, "Time conflict cancelled for " + patientName);
                _notificationService.ShowError("Visit cancelled due to time conflict.");
                return;
            }
        }

        var visit = new PatientVisit
        {
            PatientName = patientName,
            VisitDate = visitDate,
            VisitType = visitType,
            Note = note,
            DoctorName = doctorName,
            DurationInMinutes = duration
        };

        var addCommand = new AddVisitCommand(_visitManager, visit);
        _commandManager.ExecuteCommand(addCommand);

        _logger.LogActivity("Add Visit", true, "Visit added for " + patientName + " (ID: " + visit.Id + ")");
        _notificationService.ShowSuccess("Visit Added -> Visit id & Name for this visit are  " + visit.Id + " -> " + visit.PatientName);
    }

    public void UpdateVisit(int id, string patientName = null, DateTime? visitDate = null, string visitType = null, string note = null, string doctorName = null, int? duration = null)
    {
        PatientVisit currentVisit = _visitManager.FindById(id);

        if (currentVisit == null)
        {
            _logger.LogActivity("Update Visit", false, "Visit ID " + id + " not found");
            _notificationService.ShowError("Visit not found bro!");
            return;
        }

        var oldVisit = CloneVisit(currentVisit);
        var newVisit = CloneVisit(currentVisit);

        newVisit.PatientName = patientName ?? currentVisit.PatientName;
        newVisit.VisitDate = visitDate ?? currentVisit.VisitDate;
        newVisit.VisitType = visitType ?? currentVisit.VisitType;
        newVisit.Note = note ?? currentVisit.Note;
        newVisit.DoctorName = doctorName ?? currentVisit.DoctorName;
        newVisit.DurationInMinutes = duration ?? currentVisit.DurationInMinutes;

        var updateCommand = new UpdateVisitCommand(_visitManager, oldVisit, newVisit);
        _commandManager.ExecuteCommand(updateCommand);

        _logger.LogActivity("Update Visit", true, "Visit ID " + id + " updated");
        _notificationService.ShowSuccess("Updated successfully!");
    }

    public void DeleteVisit(int id)
    {
        PatientVisit visitToDelete = _visitManager.FindById(id);

        if (visitToDelete == null)
        {
            _logger.LogActivity("Delete Visit", false, "Visit ID " + id + " not found");
            _notificationService.ShowError("Visit not found.");
            return;
        }

        var deleteCommand = new DeleteVisitCommand(_visitManager, visitToDelete);
        _commandManager.ExecuteCommand(deleteCommand);

        _logger.LogActivity("Delete Visit", true, "Visit ID " + id + " deleted");
        _notificationService.ShowSuccess("Visit deleted.");
    }

    public void ShowVisit(int id)
    {
        var visit = _visitManager.FindById(id);
        _displayService.ShowVisit(visit);
    }

    public void ShowAllRecords()
    {
        var visits = _visitManager.GetAllVisits();
        _displayService.ShowAllRecords(visits);
    }

    public void ShowVisitsByType()
    {
        var visits = _visitManager.GetAllVisits();
        _displayService.ShowVisitsByType(visits);
    }

    public void GenerateVisitSummary(int visitId)
    {
        var visit = _visitManager.FindById(visitId);
        _displayService.GenerateVisitSummary(visit);
    }

    public void GetTotalVisitsByType()
    {
        var visits = _visitManager.GetAllVisits();
        _displayService.GetTotalVisitsByType(visits);
    }

    public void GetWeeklySummary()
    {
        var visits = _visitManager.GetAllVisits();
        _displayService.GetWeeklySummary(visits);
    }

    public void ShowFilteredRecords(string filterType, string filterValue)
    {
        List<PatientVisit> filteredVisits = new List<PatientVisit>();

        switch (filterType.ToLower())
        {
            case "doctor":
                filteredVisits = _visitManager.GetVisitsByDoctor(filterValue);
                break;
            case "type":
                filteredVisits = _visitManager.GetVisitsByType(filterValue);
                break;
            default:
                filteredVisits = _visitManager.GetAllVisits();
                break;
        }

        _displayService.ShowFilteredAndSortedRecords(filteredVisits);
    }

    public void ShowFilteredRecords(DateTime startDate, DateTime endDate)
    {
        var filteredVisits = _visitManager.GetVisitsByDateRange(startDate, endDate);
        _displayService.ShowFilteredAndSortedRecords(filteredVisits);
    }

    public void ShowSortedRecords(string sortBy)
    {
        var sortedVisits = _visitManager.GetSortedVisits(sortBy);
        _displayService.ShowFilteredAndSortedRecords(sortedVisits);
    }

    public void UndoLastAction()
    {
        _commandManager.UndoLastAction();
    }

    public void RedoLastAction()
    {
        _commandManager.RedoLastAction();
    }

    private bool CheckTimeSlotConflict(string patientName, DateTime visitDate)
    {
        var allVisits = _visitManager.GetAllVisits();
        return allVisits.Any(v => v.PatientName.ToLower() == patientName.ToLower() &&
                                 Math.Abs((v.VisitDate - visitDate).TotalMinutes) < 30);
    }

    private PatientVisit CloneVisit(PatientVisit visit)
    {
        return new PatientVisit
        {
            Id = visit.Id,
            PatientName = visit.PatientName,
            VisitDate = visit.VisitDate,
            VisitType = visit.VisitType,
            Note = visit.Note,
            DoctorName = visit.DoctorName,
            DurationInMinutes = visit.DurationInMinutes,
            Fee = visit.Fee
        };
    }
}