using System;
 
public interface IAdminService
{
    void AddVisit(string patientName, DateTime visitDate, string visitType, string note, string doctorName = "", int duration = 30);
    void UpdateVisit(int id, string patientName = null, DateTime? visitDate = null, string visitType = null, string note = null, string doctorName = null, int? duration = null);
    void DeleteVisit(int id);
    void GenerateMockData(int count = 350);
    void UndoLastAction();
    void RedoLastAction();
    void ShowVisit(int id);
    void ShowAllRecords();
    void ShowVisitsByType();
    void GenerateVisitSummary(int visitId);
    void GetTotalVisitsByType();
    void GetWeeklySummary();
    void ShowFilteredRecords(string filterType, string filterValue);
    void ShowFilteredRecords(DateTime startDate, DateTime endDate);
    void ShowSortedRecords(string sortBy);
}
 
public interface IReceptionistService
{
    void AddVisit(string patientName, DateTime visitDate, string visitType, string note, string doctorName = "", int duration = 30);
    void ShowVisit(int id);
    void ShowAllRecords();
    void ShowVisitsByType();
    void GenerateVisitSummary(int visitId);
    void GetTotalVisitsByType();
    void GetWeeklySummary();
    void ShowFilteredRecords(string filterType, string filterValue);
    void ShowFilteredRecords(DateTime startDate, DateTime endDate);
    void ShowSortedRecords(string sortBy);
}
 
public class AdminService : IAdminService
{
    private readonly VisitService _visitService;
    private readonly IVisitManager _visitManager;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;

    public AdminService(VisitService visitService, IVisitManager visitManager, INotificationService notificationService, ILogger logger)
    {
        _visitService = visitService;
        _visitManager = visitManager;
        _notificationService = notificationService;
        _logger = logger;
    }

    public void AddVisit(string patientName, DateTime visitDate, string visitType, string note, string doctorName = "", int duration = 30)
    {
        _visitService.AddVisit(patientName, visitDate, visitType, note, doctorName, duration);
    }

    public void UpdateVisit(int id, string patientName = null, DateTime? visitDate = null, string visitType = null, string note = null, string doctorName = null, int? duration = null)
    {
        _visitService.UpdateVisit(id, patientName, visitDate, visitType, note, doctorName, duration);
    }

    public void DeleteVisit(int id)
    {
        _visitService.DeleteVisit(id);
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

            AddVisit(patientName, visitDate, visitType, note, doctorName, duration);
        }

        _logger.LogActivity("Generate Mock Data", true, count + " mock entries generated");
        _notificationService.ShowSuccess("Mock data generated successfully! Total records: " + _visitManager.GetAllVisits().Count);
    }

    public void UndoLastAction()
    {
        _visitService.UndoLastAction();
    }

    public void RedoLastAction()
    {
        _visitService.RedoLastAction();
    }

    public void ShowVisit(int id)
    {
        _visitService.ShowVisit(id);
    }

    public void ShowAllRecords()
    {
        _visitService.ShowAllRecords();
    }

    public void ShowVisitsByType()
    {
        _visitService.ShowVisitsByType();
    }

    public void GenerateVisitSummary(int visitId)
    {
        _visitService.GenerateVisitSummary(visitId);
    }

    public void GetTotalVisitsByType()
    {
        _visitService.GetTotalVisitsByType();
    }

    public void GetWeeklySummary()
    {
        _visitService.GetWeeklySummary();
    }

    public void ShowFilteredRecords(string filterType, string filterValue)
    {
        _visitService.ShowFilteredRecords(filterType, filterValue);
    }

    public void ShowFilteredRecords(DateTime startDate, DateTime endDate)
    {
        _visitService.ShowFilteredRecords(startDate, endDate);
    }

    public void ShowSortedRecords(string sortBy)
    {
        _visitService.ShowSortedRecords(sortBy);
    }
}
 
public class ReceptionistService : IReceptionistService
{
    private readonly VisitService _visitService;

    public ReceptionistService(VisitService visitService)
    {
        _visitService = visitService;
    }

    public void AddVisit(string patientName, DateTime visitDate, string visitType, string note, string doctorName = "", int duration = 30)
    {
        _visitService.AddVisit(patientName, visitDate, visitType, note, doctorName, duration);
    }

    public void ShowVisit(int id)
    {
        _visitService.ShowVisit(id);
    }

    public void ShowAllRecords()
    {
        _visitService.ShowAllRecords();
    }

    public void ShowVisitsByType()
    {
        _visitService.ShowVisitsByType();
    }

    public void GenerateVisitSummary(int visitId)
    {
        _visitService.GenerateVisitSummary(visitId);
    }

    public void GetTotalVisitsByType()
    {
        _visitService.GetTotalVisitsByType();
    }

    public void GetWeeklySummary()
    {
        _visitService.GetWeeklySummary();
    }

    public void ShowFilteredRecords(string filterType, string filterValue)
    {
        _visitService.ShowFilteredRecords(filterType, filterValue);
    }

    public void ShowFilteredRecords(DateTime startDate, DateTime endDate)
    {
        _visitService.ShowFilteredRecords(startDate, endDate);
    }

    public void ShowSortedRecords(string sortBy)
    {
        _visitService.ShowSortedRecords(sortBy);
    }
}