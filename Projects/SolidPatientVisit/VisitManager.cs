using System;
using System.Collections.Generic;
using System.Linq;

public class VisitManager
{
    private List<PatientVisit> visits;
    private int nextId;
    private FileManager fileManager;
    private NotificationManager notificationManager;
    private Dictionary<string, decimal> feeRates;
    private Stack<PatientCommand> undoStack = new Stack<PatientCommand>();
    private Stack<PatientCommand> redoStack = new Stack<PatientCommand>();
    private const int MAX_HISTORY = 10;

    public VisitManager(FileManager fileManager, NotificationManager notificationManager)
    {
        visits = new List<PatientVisit>();
        nextId = 1;
        this.fileManager = fileManager;
        this.notificationManager = notificationManager;
        loadFeeRates();
        loadData();
    }

    private void loadFeeRates()
    {
        feeRates = fileManager.LoadFeeRates();
    }

    public void ExecuteCommand(PatientCommand command)
    {
        command.Execute();
        undoStack.Push(command);
        
        if (undoStack.Count > MAX_HISTORY)
        {
            var tempList = new List<PatientCommand>();
            for (int i = 0; i < MAX_HISTORY; i++)
            {
                if (undoStack.Count > 0)
                    tempList.Add(undoStack.Pop());
            }
            undoStack.Clear();
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                undoStack.Push(tempList[i]);
            }
        }

        redoStack.Clear();
    }

    public void UndoLastAction()
    {
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
            notificationManager.ShowUndoComplete();
        }
        else notificationManager.ShowNoActionsToUndo();
    }

    public void RedoLastAction()
    {
        if (redoStack.Count > 0)
        {
            var command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
            notificationManager.ShowRedoComplete();
        }
        else notificationManager.ShowNoActionsToRedo();
    }

    private bool checkTimeSlotConflict(string patientName, DateTime visitDate)
    {
        for (int i = 0; i < visits.Count; i++)
        {
            if (visits[i].PatientName.ToLower() == patientName.ToLower())
            { 
                double minutesDiff = Math.Abs((visits[i].VisitDate - visitDate).TotalMinutes);
                if(minutesDiff < 30)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private decimal calculateFee(string visitType, int duration)
    {
        if (feeRates.ContainsKey(visitType))
        {
            decimal baseRate = feeRates[visitType];
            if (duration > 30)
            {
                decimal extraTime = (duration - 30) / 15.0m;
                decimal extraTime2 = baseRate + (extraTime * (baseRate * 0.25m));
                return extraTime2;
            }
            return baseRate;
        }
        return 500;
    }

    public void AddVisit(string patientName, DateTime visitDate, string visitType, string Note, string doctorName = "", int duration = 30)
    {
        if (checkTimeSlotConflict(patientName, visitDate))
        {
            if (!notificationManager.ShowTimeConflictWarning(patientName))
            {
                fileManager.LogActivity("Add Visit", false, "Time conflict cancelled for " + patientName);
                notificationManager.ShowVisitCancelledDueToConflict();
                return;
            }
        }

        PatientVisit visit = new PatientVisit();
        visit.Id = nextId++;
        visit.PatientName = patientName;
        visit.VisitDate = visitDate;
        visit.VisitType = visitType;
        visit.Note = Note;
        visit.DoctorName = doctorName;
        visit.DurationInMinutes = duration;
        visit.Fee = calculateFee(visitType, duration);

        var addCmd = new AddVisitCommand(this, visit);   
        ExecuteCommand(addCmd);                          

        fileManager.LogActivity("Add Visit", true, "Visit added for " + patientName + " (ID: " + visit.Id + ")");
        notificationManager.ShowVisitAdded(visit.Id, visit.PatientName);
    }

    public void AddVisit_nosave(PatientVisit visit)
    {
        visits.Add(visit);
    }

    public void UpdateVisit(int id, string patientName = null, DateTime? visitDate = null, string visitType = null, string Note = null, string doctorName = null, int? duration = null)
    {
        PatientVisit currentVisit = visits.FirstOrDefault(v => v.Id == id);

        if (currentVisit == null)
        {
            fileManager.LogActivity("Update Visit", false, "Visit ID " + id + " not found");
            notificationManager.ShowVisitNotFound();
            return;
        }
     
        PatientVisit oldVisit = new PatientVisit
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

        PatientVisit newVisit = new PatientVisit
        {
            Id = currentVisit.Id,
            PatientName = patientName ?? currentVisit.PatientName,
            VisitDate = visitDate ?? currentVisit.VisitDate,
            VisitType = visitType ?? currentVisit.VisitType,
            Note = Note ?? currentVisit.Note,
            DoctorName = doctorName ?? currentVisit.DoctorName,
            DurationInMinutes = duration ?? currentVisit.DurationInMinutes,
            Fee = currentVisit.Fee
        };

        if (duration.HasValue)
        {
            newVisit.Fee = calculateFee(newVisit.VisitType, duration.Value);
        }
     
        var updateCmd = new UpdateVisitCommand(this, oldVisit, newVisit);
        ExecuteCommand(updateCmd);

        fileManager.LogActivity("Update Visit", true, "Visit ID " + id + " updated");
        notificationManager.ShowUpdateSuccessful();
    }

    public void DeleteVisit(int id)
    { 
        PatientVisit visitToDelete = visits.FirstOrDefault(v => v.Id == id);

        if (visitToDelete == null)
        {
            fileManager.LogActivity("Delete Visit", false, "Visit ID " + id + " not found");
            notificationManager.ShowVisitNotFound();
            return;
        }
     
        var deleteCmd = new DeleteVisitCommand(this, visitToDelete);
     
        ExecuteCommand(deleteCmd);
     
        fileManager.LogActivity("Delete Visit", true, "Visit ID " + id + " deleted");
        notificationManager.ShowVisitDeleted();
    }

    public void RemoveVisitFromList(int id)
    {
        for (int i = 0; i < visits.Count; i++)
        {
            if (visits[i].Id == id)
            {
                visits.RemoveAt(i);
                break;
            }
        }
    }

    public void UpdateVisitDirect(PatientVisit updatedVisit)
    {
        for (int i = 0; i < visits.Count; i++)
        {
            if (visits[i].Id == updatedVisit.Id)
            {
                visits[i].PatientName = updatedVisit.PatientName;
                visits[i].VisitDate = updatedVisit.VisitDate;
                visits[i].VisitType = updatedVisit.VisitType;
                visits[i].Note = updatedVisit.Note;
                visits[i].DoctorName = updatedVisit.DoctorName;
                visits[i].DurationInMinutes = updatedVisit.DurationInMinutes;
                visits[i].Fee = updatedVisit.Fee;
                break;
            }
        }
    }

    public void saveDataPublic()
    {
        fileManager.SaveVisits(visits);
    }

    public PatientVisit findById(int id)
    {
        for (int i = 0; i < visits.Count; i++)
        {
            if (visits[i].Id == id)
                return visits[i];
        }
        return null;
    }

    public List<PatientVisit> getAllVisits()
    {
        return visits;
    }

    public List<PatientVisit> getVisitsByType(string visitType)
    {
        List<PatientVisit> result = new List<PatientVisit>();

        for (int i = 0; i < visits.Count; i++)
        {
            if (visits[i].VisitType.ToLower().Contains(visitType.ToLower()))
            {
                result.Add(visits[i]);
            }
        }
        return result;
    }

    public List<PatientVisit> getVisitsByDoctor(string doctorName)
    {
        List<PatientVisit> result = new List<PatientVisit>();

        for(int i = 0; i<visits.Count; i++)
        {
            if (visits[i] != null && visits[i].DoctorName.ToLower().Contains(doctorName.ToLower()))
                {
                result.Add(visits[i]);
            }
        }

        return result;
    }

    public List<PatientVisit> getVisitsByDateRange(DateTime startDate, DateTime endDate)
    {
        List<PatientVisit> result = new List<PatientVisit>();

        for(int i = 0; i < visits.Count; i++)
        {
            if (visits[i].VisitDate >= startDate && visits[i].VisitDate <= endDate)
            {
                result.Add(visits[i]);
            }
        }
        return result;
    }

    public List<PatientVisit> GetSortedVisits(string sortBy)
    {
        List<PatientVisit> sortedList = new List<PatientVisit>(visits);

        switch (sortBy.ToLower())
        {
            case "date":
                sortedList = sortedList.OrderBy(visit => visit.VisitDate).ToList();
                break;

            case "name":
                sortedList = sortedList.OrderBy(visit => visit.PatientName).ToList();
                break;

            case "type":
                sortedList = sortedList.OrderBy(visit => visit.VisitType).ToList();
                break;

            case "fee":
                sortedList = sortedList.OrderBy(visit => visit.Fee).ToList();
                break;

            default: 
                sortedList = sortedList.OrderBy(visit => visit.VisitDate).ToList();
                break;
        }

        return sortedList;
    }

    public void generateMockData(int count = 350)
    {
        string[] visitTypes = { "Checkup", "Emergency", "Follow-up", "Consultation", "Surgery", "Lab Test", "X-Ray", "Vaccination", "Physical Therapy", "Dental" };
        string[] patientNames = { "Ahmed Ali", "Sara Khan", "Muhammad Hassan", "Fatima Sheikh", "Ali Raza", "Ayesha Malik", "Usman Ahmed", "Zainab Hussain", "Hassan Ali", "Mariam Qureshi", "Bilal Shah", "Sana Tariq", "Imran Khan", "Nida Zahra", "Farhan Malik" };
        string[] doctorNames = { "Dr. Ahmad", "Dr. Fatima", "Dr. Hassan", "Dr. Ayesha", "Dr. Ali", "Dr. Sara", "Dr. Usman", "Dr. Zainab", "Dr. Bilal", "Dr. Mariam" };

        Random rand = new Random();
        notificationManager.ShowGeneratingMockData(count);

        for (int i = 0; i < count; i++)
        {
            string patientName = patientNames[rand.Next(patientNames.Length)];
            string visitType = visitTypes[rand.Next(visitTypes.Length)];
            string doctorName = rand.Next(100) < 80 ? doctorNames[rand.Next(doctorNames.Length)] : "";

            DateTime visitDate = DateTime.Now.AddDays(-rand.Next(365));

            string[] Notes = {
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
            string Note = Notes[rand.Next(Notes.Length)];

            int duration = rand.Next(15, 91);

            PatientVisit visit = new PatientVisit();
            visit.Id = nextId++;
            visit.PatientName = patientName;
            visit.VisitDate = visitDate;
            visit.VisitType = visitType;
            visit.Note = Note;
            visit.DoctorName = doctorName;
            visit.DurationInMinutes = duration;
            visit.Fee = calculateFee(visitType, duration);

            visits.Add(visit);
        }

        fileManager.SaveVisits(visits);
        fileManager.LogActivity("Generate Mock Data", true, count + " mock entries generated");
        notificationManager.ShowMockDataGenerated(count, visits.Count);
    }

    private void loadData()
    {
        var loadedVisits = fileManager.LoadVisits();
        visits = loadedVisits.visits;
        nextId = loadedVisits.nextId;
    }
}