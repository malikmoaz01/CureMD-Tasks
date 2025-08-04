using System;
using System.Collections.Generic;
using System.Linq; 

public interface IVisitManager
{
    void AddVisitDirect(PatientVisit visit);
    void RemoveVisitFromList(int id);
    void UpdateVisitDirect(PatientVisit updatedVisit);
    void SaveData();
    PatientVisit FindById(int id);
    List<PatientVisit> GetAllVisits();
    List<PatientVisit> GetVisitsByType(string visitType);
    List<PatientVisit> GetVisitsByDoctor(string doctorName);
    List<PatientVisit> GetVisitsByDateRange(DateTime startDate, DateTime endDate);
    List<PatientVisit> GetSortedVisits(string sortBy);
}
 
public class VisitManager : IVisitManager
{
    private List<PatientVisit> _visits;
    private int _nextId;
    private readonly IVisitRepository _repository;
    private readonly ILogger _logger;
    private readonly INotificationService _notificationService;
    private readonly IFeeCalculator _feeCalculator;

    public VisitManager(IVisitRepository repository, ILogger logger, INotificationService notificationService, IFeeCalculator feeCalculator)
    {
        _repository = repository;
        _logger = logger;
        _notificationService = notificationService;
        _feeCalculator = feeCalculator;
        _visits = new List<PatientVisit>();
        _nextId = 1;
        LoadData();
    }

    private void LoadData()
    {
        _visits = _repository.LoadVisits();
        if (_visits.Count > 0)
        {
            _nextId = _visits.Max(v => v.Id) + 1;
        }
    }

    public void SaveData()
    {
        _repository.SaveVisits(_visits);
    }

    public void AddVisitDirect(PatientVisit visit)
    {
        if (visit.Id == 0)
        {
            visit.Id = _nextId++;
        }
        _visits.Add(visit);
    }

    public void RemoveVisitFromList(int id)
    {
        for (int i = 0; i < _visits.Count; i++)
        {
            if (_visits[i].Id == id)
            {
                _visits.RemoveAt(i);
                break;
            }
        }
    }

    public void UpdateVisitDirect(PatientVisit updatedVisit)
    {
        for (int i = 0; i < _visits.Count; i++)
        {
            if (_visits[i].Id == updatedVisit.Id)
            {
                _visits[i] = updatedVisit;
                break;
            }
        }
    }

    public PatientVisit FindById(int id)
    {
        return _visits.FirstOrDefault(v => v.Id == id);
    }

    public List<PatientVisit> GetAllVisits()
    {
        return new List<PatientVisit>(_visits);
    }

    public List<PatientVisit> GetVisitsByType(string visitType)
    {
        return _visits.Where(v => v.VisitType.ToLower().Contains(visitType.ToLower())).ToList();
    }

    public List<PatientVisit> GetVisitsByDoctor(string doctorName)
    {
        return _visits.Where(v => v.DoctorName != null && v.DoctorName.ToLower().Contains(doctorName.ToLower())).ToList();
    }

    public List<PatientVisit> GetVisitsByDateRange(DateTime startDate, DateTime endDate)
    {
        return _visits.Where(v => v.VisitDate >= startDate && v.VisitDate <= endDate).ToList();
    }

    public List<PatientVisit> GetSortedVisits(string sortBy)
    {
        var sortedList = new List<PatientVisit>(_visits);

        switch (sortBy.ToLower())
        {
            case "date":
                return sortedList.OrderBy(visit => visit.VisitDate).ToList();
            case "name":
                return sortedList.OrderBy(visit => visit.PatientName).ToList();
            case "type":
                return sortedList.OrderBy(visit => visit.VisitType).ToList();
            case "fee":
                return sortedList.OrderBy(visit => visit.Fee).ToList();
            default:
                return sortedList.OrderBy(visit => visit.VisitDate).ToList();
        }
    }

    private bool CheckTimeSlotConflict(string patientName, DateTime visitDate)
    {
        return _visits.Any(v => v.PatientName.ToLower() == patientName.ToLower() && 
                              Math.Abs((v.VisitDate - visitDate).TotalMinutes) < 30);
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
            Id = _nextId++,
            PatientName = patientName,
            VisitDate = visitDate,
            VisitType = visitType,
            Note = note,
            DoctorName = doctorName,
            DurationInMinutes = duration,
            Fee = _feeCalculator.CalculateFee(visitType, duration)
        };

        _visits.Add(visit);
        SaveData();

        _logger.LogActivity("Add Visit", true, "Visit added for " + patientName + " (ID: " + visit.Id + ")");
        _notificationService.ShowSuccess("Visit Added -> Visit id & Name for this visit are  " + visit.Id + " -> " + visit.PatientName);
    }

    public void UpdateVisit(int id, string patientName = null, DateTime? visitDate = null, string visitType = null, string note = null, string doctorName = null, int? duration = null)
    {
        PatientVisit currentVisit = FindById(id);

        if (currentVisit == null)
        {
            _logger.LogActivity("Update Visit", false, "Visit ID " + id + " not found");
            _notificationService.ShowError("Visit not found bro!");
            return;
        }

        var oldVisit = new PatientVisit
        {
            Id = currentVisit.Id,
            PatientName = currentVisit.PatientName,
            VisitDate = currentVisit.VisitDate,
            VisitType = currentVisit.VisitType,
            Note = currentVisit.Note,
            DoctorName = currentVisit.DoctorName,
            DurationInMinutes = currentVisit.DurationInMinutes,
            Fee = currentVisit.Fee
        };

        currentVisit.PatientName = patientName ?? currentVisit.PatientName;
        currentVisit.VisitDate = visitDate ?? currentVisit.VisitDate;
        currentVisit.VisitType = visitType ?? currentVisit.VisitType;
        currentVisit.Note = note ?? currentVisit.Note;
        currentVisit.DoctorName = doctorName ?? currentVisit.DoctorName;
        currentVisit.DurationInMinutes = duration ?? currentVisit.DurationInMinutes;

        if (duration.HasValue)
        {
            currentVisit.Fee = _feeCalculator.CalculateFee(currentVisit.VisitType, duration.Value);
        }

        SaveData();
        _logger.LogActivity("Update Visit", true, "Visit ID " + id + " updated");
        _notificationService.ShowSuccess("Updated successfully!");
    }

    public void DeleteVisit(int id)
    {
        PatientVisit visitToDelete = FindById(id);

        if (visitToDelete == null)
        {
            _logger.LogActivity("Delete Visit", false, "Visit ID " + id + " not found");
            _notificationService.ShowError("Visit not found.");
            return;
        }

        RemoveVisitFromList(id);
        SaveData();

        _logger.LogActivity("Delete Visit", true, "Visit ID " + id + " deleted");
        _notificationService.ShowSuccess("Visit deleted.");
    }

    public void GenerateMockData(int count = 350)
    {
        Random rand = new Random();
        _notificationService.ShowInfo("Generating " + count + " mock entries...");

        string[] patientNames = { "Ahmed Ali", "Sara Khan", "Muhammad Hassan", "Fatima Sheikh", "Ali Raza", "Ayesha Malik", "Usman Ahmed", "Zainab Hussain", "Hassan Ali", "Mariam Qureshi", "Bilal Shah", "Sana Tariq", "Imran Khan", "Nida Zahra", "Farhan Malik" };
        string[] visitTypes = { "Checkup", "Emergency", "Follow-up", "Consultation", "Surgery", "Lab Test", "X-Ray", "Vaccination", "Physical Therapy", "Dental" };
        string[] doctorNames = { "Dr. Ahmad", "Dr. Fatima", "Dr. Hassan", "Dr. Ayesha", "Dr. Ali", "Dr. Sara", "Dr. Usman", "Dr. Zainab", "Dr. Bilal", "Dr. Mariam" };

        for (int i = 0; i < count; i++)
        {
            string patientName = patientNames[rand.Next(patientNames.Length)];
            string visitType = visitTypes[rand.Next(visitTypes.Length)];
            string doctorName = rand.Next(100) < 80 ? doctorNames[rand.Next(doctorNames.Length)] : "";

            DateTime visitDate = DateTime.Now.AddDays(-rand.Next(365));

            string[] notes = {
                "Regular checkup completed",
                "Patient complained of headache",
                "Follow up for previous treatment",
                "Emergency visit - fever",
                "Routine blood test",
                "X-ray examination done",
                "Vaccination administered",
                "Physical therapy session",
                "Consultation for back pain",
                "Lab results reviewed"
            };
            string note = notes[rand.Next(notes.Length)];

            int duration = rand.Next(15, 91);

            var visit = new PatientVisit
            {
                Id = _nextId++,
                PatientName = patientName,
                VisitDate = visitDate,
                VisitType = visitType,
                Note = note,
                DoctorName = doctorName,
                DurationInMinutes = duration,
                Fee = _feeCalculator.CalculateFee(visitType, duration)
            };

            _visits.Add(visit);
        }

        SaveData();
        _logger.LogActivity("Generate Mock Data", true, count + " mock entries generated");
        _notificationService.ShowSuccess("Mock data generated successfully! Total records: " + _visits.Count);
    }
}