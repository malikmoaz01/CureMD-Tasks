using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public enum UserRole
{
    Admin,
    Receptionist
}

public class PatientVisit
{

    public int Id;
    public string PatientName;
    public DateTime VisitDate;
    public string VisitType;
    public string Note;
    public string DoctorName;
    public int DurationInMinutes;
    public decimal Fee;

}

public interface PatientCommand
{
    void Execute();
    void Undo();
}

public class AddVisitCommand : PatientCommand
{
    private PatientVisitManager patientvisitmanageobj;
    private PatientVisit patientvisitobj;

    public AddVisitCommand(PatientVisitManager patientvisitmanageobj, PatientVisit patientvisitobj)
    {
        this.patientvisitmanageobj = patientvisitmanageobj;
        this.patientvisitobj = patientvisitobj;
    }
    public void Execute()
    {
        patientvisitmanageobj.AddVisit_nosave(patientvisitobj);
        patientvisitmanageobj.saveDataPublic();
    }
    public void Undo()
    {
        patientvisitmanageobj.RemoveVisitFromList(patientvisitobj.Id);
        patientvisitmanageobj.saveDataPublic();
    }
}


public class UpdateVisitCommand : PatientCommand
{
    private PatientVisitManager manager;
    private PatientVisit oldVisit;
    private PatientVisit newVisit;

    public UpdateVisitCommand(PatientVisitManager manager, PatientVisit oldVisit, PatientVisit newVisit)
    {
        this.manager = manager;
        this.oldVisit = new PatientVisit
        {
            Id = oldVisit.Id,
            PatientName = oldVisit.PatientName,
            VisitDate = oldVisit.VisitDate,
            VisitType = oldVisit.VisitType,
            Note = oldVisit.Note,
            DoctorName = oldVisit.DoctorName,
            DurationInMinutes = oldVisit.DurationInMinutes,
            Fee = oldVisit.Fee
        };
        this.newVisit = new PatientVisit
        {
            Id = newVisit.Id,
            PatientName = newVisit.PatientName,
            VisitDate = newVisit.VisitDate,
            VisitType = newVisit.VisitType,
            Note = newVisit.Note,
            DoctorName = newVisit.DoctorName,
            DurationInMinutes = newVisit.DurationInMinutes,
            Fee = newVisit.Fee
        };
    }

    public void Execute()
    {
        manager.UpdateVisitDirect(newVisit);
        manager.saveDataPublic();
    }

    public void Undo()
    {
        manager.UpdateVisitDirect(oldVisit);
        manager.saveDataPublic();
    }
}


public class DeleteVisitCommand : PatientCommand
{
    private PatientVisitManager manager;
    private PatientVisit visit;

    public DeleteVisitCommand(PatientVisitManager manager, PatientVisit visit)
    {
        this.manager = manager;
        this.visit = new PatientVisit
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

    public void Execute()
    {
        manager.RemoveVisitFromList(visit.Id);
        manager.saveDataPublic();
    }

    public void Undo()
    {
        manager.AddVisit_nosave(visit);
        manager.saveDataPublic();
    }
}







public class ActivityLogger
{
    private string logFilePath = "activity_log.txt";

    public void LogActivity(string action, bool success, string details = "")
    {
        try
        {
            string logEntry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") 
                       + 
                            " | " 
                       +
                             action 
                       + 
                             " | " 
                       + 
                             (success ? "SUCCESS" : "FAILURE") 
                       +
                             " | " 
                       +
                             details;

            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error logging activity: " + ex.Message);
        }
    }
}













public class PatientVisitManager
{


    private List<PatientVisit> visits;
    
    private int nextId;
    
    private string filePath;


    private ActivityLogger logger;

    private Dictionary<string, decimal> feeRates;

    
    
    private string[] visitTypes = { "Checkup", "Emergency", "Follow-up", "Consultation", "Surgery", "Lab Test", "X-Ray", "Vaccination", "Physical Therapy", "Dental" };
    
    private string[] patientNames = { "Ahmed Ali", "Sara Khan", "Muhammad Hassan", "Fatima Sheikh", "Ali Raza", "Ayesha Malik", "Usman Ahmed", "Zainab Hussain", "Hassan Ali", "Mariam Qureshi", "Bilal Shah", "Sana Tariq", "Imran Khan", "Nida Zahra", "Farhan Malik" };
    
    private string[] doctorNames = { "Dr. Ahmad", "Dr. Fatima", "Dr. Hassan", "Dr. Ayesha", "Dr. Ali", "Dr. Sara", "Dr. Usman", "Dr. Zainab", "Dr. Bilal", "Dr. Mariam" };





    private Stack<PatientCommand> undoStack = new Stack<PatientCommand>();
    private Stack<PatientCommand> redoStack = new Stack<PatientCommand>();
    private const int MAX_HISTORY = 10;




    public PatientVisitManager(string fileName = "patient_visits.txt")
    {
        visits = new List<PatientVisit>();

        nextId = 1;

        filePath = fileName;

        logger = new ActivityLogger();

        loadFeeRates();

        loadData();

    }










    


    private void loadFeeRates()
    {
        string feeFilePath = "fees.json";

        try
        {
            if (File.Exists(feeFilePath))
            
            {
                string json = File.ReadAllText(feeFilePath);
                feeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
            
            }
            
            else
            
            {  
                feeRates = new Dictionary<string, decimal>()
            
                {
                    {"Consultation", 500},
                    {"Follow-up", 300},
                    {"Emergency", 1000}
                };
                
                saveFeeRates();
            
            }

        }
        catch (Exception)
        {
            feeRates = new Dictionary<string, decimal>()
            
            {
            
                {"Consultation", 500},
                {"Follow-up", 300},
                {"Emergency", 1000}
            
            };
        }
    }

    private void saveFeeRates()
    {
        try
        {
            string json = JsonSerializer.Serialize(feeRates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("fees.json", json);
        
        }
        
        catch (Exception ex)
        
        {

            Console.WriteLine("Error saving fee rates: " + ex.Message);

        }
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
        Console.WriteLine("Undo completed.");
    }
    else Console.WriteLine("No actions to undo.");
}

public void RedoLastAction()
{
    if (redoStack.Count > 0)
    {
        var command = redoStack.Pop();
        command.Execute();
        undoStack.Push(command);
        Console.WriteLine("Redo completed.");
    }
    else Console.WriteLine("No actions to redo.");
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
    saveData();
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
    





public void AddVisit(string patientName, DateTime visitDate, string visitType, string Note, string doctorName = "", int duration = 30)
{
    if (checkTimeSlotConflict(patientName, visitDate))
    {
        Console.WriteLine("Warning: This patient has another visit within 30 minutes. Proceed? (Y/N)");
        string response = Console.ReadLine();
        if (response.ToUpper() != "Y")
        {
            logger.LogActivity("Add Visit", false, "Time conflict cancelled for " + patientName);
            Console.WriteLine("Visit cancelled due to time conflict.");
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

    logger.LogActivity("Add Visit", true, "Visit added for " + patientName + " (ID: " + visit.Id + ")");
    Console.WriteLine("Visit Added -> Visit id & Name for this visit are  " + visit.Id + " -> " + visit.PatientName);
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
        logger.LogActivity("Update Visit", false, "Visit ID " + id + " not found");
        Console.WriteLine("Visit not found bro!");
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

    logger.LogActivity("Update Visit", true, "Visit ID " + id + " updated");
    Console.WriteLine("Updated successfully!");
}






public void DeleteVisit(int id)
{ 
    PatientVisit visitToDelete = visits.FirstOrDefault(v => v.Id == id);

    if (visitToDelete == null)
    {
        logger.LogActivity("Delete Visit", false, "Visit ID " + id + " not found");
        Console.WriteLine("Visit not found.");
        return;
    }
 
    var deleteCmd = new DeleteVisitCommand(this, visitToDelete);
 
    ExecuteCommand(deleteCmd);
 
    logger.LogActivity("Delete Visit", true, "Visit ID " + id + " deleted");
    Console.WriteLine("Visit deleted.");
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







    public void showVisit(PatientVisit visit)
    {
        if (visit == null)
        {
            Console.WriteLine("No visit found ");
            return;
        }

        Console.WriteLine("\n=======================================================\n");
        Console.WriteLine("ID: " + visit.Id);
        Console.WriteLine("Patient: " + visit.PatientName);
        Console.WriteLine("Date: " + visit.VisitDate.ToString("yyyy-MM-dd"));
        Console.WriteLine("Type: " + visit.VisitType);
        Console.WriteLine("Note: " + visit.Note);
        Console.WriteLine("Doctor: " + (string.IsNullOrEmpty(visit.DoctorName) ? "Not specified" : visit.DoctorName));
        Console.WriteLine("Duration: " + visit.DurationInMinutes + " minutes");
        Console.WriteLine("Fee: Rs. " + visit.Fee);
        Console.WriteLine("\n=======================================================\n");
    }







    public void showAllRecords()
    {
        if (visits.Count == 0)
        {
            Console.WriteLine("No records found");
            return;
        }

        Console.WriteLine("\n ===== Patients Records ======\n");

        for (int i = 0; i < visits.Count; i++)
        {
            PatientVisit v = visits[i];
            Console.WriteLine($"{v.Id} | {v.PatientName} | {v.VisitDate:yyyy-MM-dd HH:mm:ss} | {v.VisitType} | {v.Note} | {v.DoctorName} | {v.DurationInMinutes}min | Rs.{v.Fee}");
        }

        Console.WriteLine("\n===============================\n");
    }






    public void showFilteredAndSortedRecords(List<PatientVisit> visitList)
    {
        if (visitList.Count == 0)
        {
            Console.WriteLine("No records found matching criteria");
            return;
        }

        Console.WriteLine("\n ===== Filtered/Sorted Records ======\n");

        for (int i = 0; i < visitList.Count; i++)
        {
            PatientVisit v = visitList[i];
            Console.WriteLine($"{v.Id} | {v.PatientName} | {v.VisitDate:yyyy-MM-dd} | {v.VisitType} | {v.Note} | {v.DoctorName} | {v.DurationInMinutes}min | Rs.{v.Fee}");
        }

        Console.WriteLine("\n===============================\n");
    }





    public void showvisitsbytype()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();

        for (int i = 0; i < visits.Count; i++)
        {
            string type = visits[i].VisitType;
            if (dict.ContainsKey(type))
                dict[type]++;
            else
                dict[type] = 1;
        }

        Console.WriteLine("\n ===== Patients are showing by Visit Type ====");

        foreach (var item in dict)
        {
            Console.WriteLine(item.Key + ": " + item.Value + " visits");
        }

        Console.WriteLine("\n=================================\n");
    }






    public void generateVisitSummary(int visitId)
    {
        PatientVisit generatevisitobj = findById(visitId);

        if (generatevisitobj == null)
        {
            Console.WriteLine("Visit not found!");
            return;
        }

        Console.WriteLine("\n=== Individual Visit Summary ===");

        Console.WriteLine("Visit ID: " + generatevisitobj.Id);
        Console.WriteLine("Patient Name: " + generatevisitobj.PatientName);
        Console.WriteLine("Visit Date: " + generatevisitobj.VisitDate.ToString("dddd, MMMM dd, yyyy"));
        Console.WriteLine("Visit Type: " + generatevisitobj.VisitType);
        Console.WriteLine("Notes: " + generatevisitobj.Note);
        Console.WriteLine("Attending Doctor: " + (string.IsNullOrEmpty(generatevisitobj.DoctorName) ? "Not assigned" : generatevisitobj.DoctorName));
        Console.WriteLine("Duration: " + generatevisitobj.DurationInMinutes + " minutes");
        Console.WriteLine("Fee: Rs. " + generatevisitobj.Fee);

        Console.WriteLine("\n=================================\n");
    }






    public void getTotalVisitsByType()
    {
        Dictionary<string, int> dict_count = new Dictionary<string, int>();

        for (int i = 0; i < visits.Count; i++)
        {
            string type = visits[i].VisitType;
            if (dict_count.ContainsKey(type))
                dict_count[type]++;
            else
                dict_count[type] = 1;
        }

        Console.WriteLine("\n*** Total Visits by Type ***");

        int totalVisits = 0;

        foreach (var pair in dict_count)
        {
            Console.WriteLine(pair.Key + ": " + pair.Value + " visits");
            totalVisits += pair.Value;
        }
        Console.WriteLine("Total Overall: " + totalVisits + " visits");

        Console.WriteLine("\n=================================\n");
    }







    public void getWeeklySummary()
    {
        DateTime today = DateTime.Now;
        DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
        DateTime weekEnd = weekStart.AddDays(7);

        List<PatientVisit> weeklyVisits = new List<PatientVisit>();

        for (int i = 0; i < visits.Count; i++)
        {
            if (visits[i].VisitDate >= weekStart && visits[i].VisitDate < weekEnd)
            {
                weeklyVisits.Add(visits[i]);
            }
        }

        Console.WriteLine("\n*** Weekly Visit Summary ***");

        Console.WriteLine("Week: " + weekStart.ToString("MMM dd") + " - " + weekEnd.AddDays(-1).ToString("MMM dd, yyyy"));

        Console.WriteLine("Total visits this week: " + weeklyVisits.Count);

        Dictionary<string, int> dailyCounts = new Dictionary<string, int>();

        for (int i = 0; i < weeklyVisits.Count; i++)
        {
            string day = weeklyVisits[i].VisitDate.ToString("dddd");
            if (dailyCounts.ContainsKey(day))
                dailyCounts[day]++;
            else
                dailyCounts[day] = 1;
        }

        Console.WriteLine("\nDaily breakdown:");

        string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        for (int i = 0; i < days.Length; i++)
        {
            int count = dailyCounts.ContainsKey(days[i]) ? dailyCounts[days[i]] : 0;
            Console.WriteLine(days[i] + ": " + count + " visits");
        }

        Console.WriteLine("****************************");
    }







    public void generateMockData(int count = 350)
    {
        Random rand = new Random();
        Console.WriteLine("Generating " + count + " mock entries...");

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

        saveData();
        logger.LogActivity("Generate Mock Data", true, count + " mock entries generated");
        Console.WriteLine("Mock data generated successfully! Total records: " + visits.Count);
    }








    private void saveData()
    {
        try
        {
            StreamWriter writer = new StreamWriter(filePath);
            for (int i = 0; i < visits.Count; i++)
            {
                PatientVisit visit = visits[i];
                string line = visit.Id + "|" + visit.PatientName + "|" +
                             visit.VisitDate.ToString("yyyy-MM-dd HH:mm:ss") + "|" +
                             visit.VisitType + "|" + visit.Note + "|" + visit.DoctorName + "|" +
                             visit.DurationInMinutes + "|" + visit.Fee;
                writer.WriteLine(line);
            }
            writer.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }






    private void loadData()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 5)
                        {
                            PatientVisit visit = new PatientVisit();
                            visit.Id = int.Parse(parts[0]);
                            visit.PatientName = parts[1];
                            visit.VisitDate = DateTime.Parse(parts[2]);
                            visit.VisitType = parts[3];
                            visit.Note = parts[4];
                            visit.DoctorName = parts.Length > 5 ? parts[5] : "";
                            visit.DurationInMinutes = parts.Length > 6 ? int.Parse(parts[6]) : 30;
                            visit.Fee = parts.Length > 7 ? decimal.Parse(parts[7]) : calculateFee(visit.VisitType, visit.DurationInMinutes);

                            visits.Add(visit);
                            if (visit.Id >= nextId)
                                nextId = visit.Id + 1;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
        }
    }
}

public class Program
{
    private static UserRole currentUserRole;

    public static void Main()
    {
        if (!authenticateUser())
        {
            Console.WriteLine("Authentication failed. Exiting...");
            return;
        }

        PatientVisitManager manager = new PatientVisitManager();

        while (true)
        {
            showMenu();
            string input = Console.ReadLine();

            if (input == "1")
            {
                addNewVisit(manager);
            }
            else if (input == "2" && currentUserRole == UserRole.Admin)
            {
                updateVisit(manager);
            }
            else if (input == "3" && currentUserRole == UserRole.Admin)
            {
                deleteVisit(manager);
            }
            else if (input == "4")
            {
                searchById(manager);
            }
            else if (input == "5")
            {
                manager.showAllRecords();
            }
            else if (input == "6")
            {
                manager.showvisitsbytype();
            }
            else if (input == "7")
            {
                generateIndividualSummary(manager);
            }
            else if (input == "8")
            {
                manager.getTotalVisitsByType();
            }
            else if (input == "9")
            {
                manager.getWeeklySummary();
            }
            else if (input == "10" && currentUserRole == UserRole.Admin)
            {
                generateMockData(manager);
            }
            else if (input == "11")
            {
                filterAndSortMenu(manager);
            }
            else if (input == "12")
            {
                Environment.Exit(0);
            }
            else if (input == "13" && currentUserRole == UserRole.Admin)
            {
                manager.UndoLastAction();
            }
            else if (input == "14" && currentUserRole == UserRole.Admin)
            {
                manager.RedoLastAction();
            }
            else
            {
                Console.WriteLine("Wrong choice! Try again.");
            }
        }
    }

    private static bool authenticateUser()
    {
        Console.WriteLine("\n==== PVMS Login ====");
        Console.WriteLine("1. Admin");
        Console.WriteLine("2. Receptionist");
        Console.Write("Select role: ");

        string roleChoice = Console.ReadLine();

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        if (roleChoice == "1" && username == "curemd" && password == "curemd")
        {
            currentUserRole = UserRole.Admin;
            Console.WriteLine("Admin login successful!");
            return true;
        }
        else if (roleChoice == "2" && username == "curemd1" && password == "curemd1")
        {
            currentUserRole = UserRole.Receptionist;
            Console.WriteLine("Receptionist login successful!");
            return true;
        }

        return false;
    }






    private static void showMenu()
    {
        Console.WriteLine("\n ==== Welcome to Patient Visit MS by Malik Moaz ====");
        Console.WriteLine("1. Add New Visit");

        if (currentUserRole == UserRole.Admin)
        {
            Console.WriteLine("2. Update Visit");
            Console.WriteLine("3. Delete Visit");
        }

        Console.WriteLine("4. Search by ID");
        Console.WriteLine("5. Show All Records");
        Console.WriteLine("6. Show Visits by Category");
        Console.WriteLine("7. Individual Visit Summary");
        Console.WriteLine("8. Total Visits by Type");
        Console.WriteLine("9. Weekly Visit Summary");

        if (currentUserRole == UserRole.Admin)
        {
            Console.WriteLine("10. Generate Mock Data");
            Console.WriteLine("13. Undo Last Action");
            Console.WriteLine("14. Redo Last Action");
        }

        Console.WriteLine("11. Filter & Sort Records");
        Console.WriteLine("12. Exit");

        Console.Write("Choose option: ");
    }

    private static void filterAndSortMenu(PatientVisitManager manager)
    {
        Console.WriteLine("\n==== Filter & Sort Menu ====");
        Console.WriteLine("1. Filter by Doctor");
        Console.WriteLine("2. Filter by Visit Type");
        Console.WriteLine("3. Filter by Date Range");
        Console.WriteLine("4. Sort by Date");
        Console.WriteLine("5. Sort by Name");
        Console.WriteLine("6. Sort by Type");
        Console.WriteLine("7. Sort by Fee");
        Console.Write("Choose option: ");

        string choice = Console.ReadLine();
        List<PatientVisit> filteredVisits = new List<PatientVisit>();

        if (choice == "1")
        {
            Console.Write("Enter doctor name: ");
            string doctorName = Console.ReadLine();
            filteredVisits = manager.getVisitsByDoctor(doctorName);
        }
        else if (choice == "2")
        {
            Console.Write("Enter visit type: ");
            string visitType = Console.ReadLine();
            filteredVisits = manager.getVisitsByType(visitType);
        }
        else if (choice == "3")
        {
            Console.Write("Enter start date (yyyy-mm-dd): ");
            DateTime startDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter end date (yyyy-mm-dd): ");
            DateTime endDate = DateTime.Parse(Console.ReadLine());
            filteredVisits = manager.getVisitsByDateRange(startDate, endDate);
        }
        else if (choice == "4")
        {
            filteredVisits = manager.GetSortedVisits("date");
        }
        else if (choice == "5")
        {
            filteredVisits = manager.GetSortedVisits("name");
        }
        else if (choice == "6")
        {
            filteredVisits = manager.GetSortedVisits("type");
        }
        else if (choice == "7")
        {
            filteredVisits = manager.GetSortedVisits("fee");
        }

        manager.showFilteredAndSortedRecords(filteredVisits);
    }




    private static void addNewVisit(PatientVisitManager obj)
    {
        Console.Write("Patient Name: ");
        string patientName = Console.ReadLine();

        Console.Write("Visit Date (yyyy-mm-dd) or Enter for today: ");
        string dateStr = Console.ReadLine();
        DateTime visitDate;

        if (string.IsNullOrEmpty(dateStr))
            visitDate = DateTime.Now;
        else
            visitDate = DateTime.Parse(dateStr);

        Console.Write("Visit Type: ");
        string visitType = Console.ReadLine();

        Console.Write("Notes: ");
        string Note = Console.ReadLine();

        Console.Write("Doctor Name (optional): ");
        string doctorName = Console.ReadLine();

        Console.Write("Duration in minutes (default 30): ");
        string durationStr = Console.ReadLine();

        int duration = string.IsNullOrEmpty(durationStr) ? 30 : int.Parse(durationStr);

        obj.AddVisit(patientName, visitDate, visitType, Note, doctorName, duration);
    }










    private static void updateVisit(PatientVisitManager manager)
    {
        Console.Write("Enter Visit ID to update: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("New Patient Name (Enter to skip): ");
        string patientName = Console.ReadLine();
        if (string.IsNullOrEmpty(patientName)) patientName = null;

        Console.Write("New Visit Date (yyyy-mm-dd) (Enter to skip): ");
        string dateStr = Console.ReadLine();
        DateTime? visitDate = string.IsNullOrEmpty(dateStr) ? null : DateTime.Parse(dateStr);

        Console.Write("New Visit Type (Enter to skip): ");
        string visitType = Console.ReadLine();
        if (string.IsNullOrEmpty(visitType)) visitType = null;

        Console.Write("New Note (Enter to skip): ");
        string note = Console.ReadLine();
        if (string.IsNullOrEmpty(note)) note = null;

        Console.Write("New Doctor Name (Enter to skip): ");
        string doctorName = Console.ReadLine();
        if (string.IsNullOrEmpty(doctorName)) doctorName = null;

        Console.Write("New Duration in minutes (Enter to skip): ");
        string durationStr = Console.ReadLine();

        int? duration = string.IsNullOrEmpty(durationStr) ? null : int.Parse(durationStr);

        manager.UpdateVisit(id, patientName, visitDate, visitType, note, doctorName, duration);
    }





    private static void deleteVisit(PatientVisitManager manager)
    {
        Console.Write("Enter Visit ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        manager.DeleteVisit(id);
    }




    private static void searchById(PatientVisitManager manager)
    {
        Console.Write("Enter Visit ID: ");

        int id = int.Parse(Console.ReadLine());

        PatientVisit visit = manager.findById(id);

        manager.showVisit(visit);
    }




    private static void generateIndividualSummary(PatientVisitManager manager)
    {
        Console.Write("Enter Visit ID for summary: ");

        int id = int.Parse(Console.ReadLine());

        manager.generateVisitSummary(id);
    }





    private static void generateMockData(PatientVisitManager manager)
    {

        Console.Write("Enter number of mock records to generate (default 350): ");

        string countStr = Console.ReadLine();

        int count = string.IsNullOrEmpty(countStr) ? 350 : int.Parse(countStr);

        manager.generateMockData(count);

    }
}