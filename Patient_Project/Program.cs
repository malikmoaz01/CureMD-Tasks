using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PatientVisit
{
    public int Id;
    public string PatientName;
    public string DoctorName;
    public DateTime VisitDate;
    public string VisitType;
    public string Notes;
}

public class PatientVisitManager
{
    private List<PatientVisit> visits;
    private int nextId;
    private string filePath;

    public PatientVisitManager(string fileName = "patient_visits.txt")
    {
        visits = new List<PatientVisit>();
        nextId = 1;
        filePath = fileName;
        loadData();
    }

    public void AddVisit(string patientName, string doctorName, DateTime visitDate, string visitType, string notes = "")
    {
        PatientVisit visit = new PatientVisit();
        visit.Id = nextId++;
        visit.PatientName = patientName;
        visit.DoctorName = doctorName;
        visit.VisitDate = visitDate;
        visit.VisitType = visitType;
        visit.Notes = notes;
        
        visits.Add(visit);
        saveData();
        Console.WriteLine("Visit added! ID: " + visit.Id);
    }

    public void UpdateVisit(int id, string patientName = null, string doctorName = null, DateTime? visitDate = null, string visitType = null, string notes = null)
    {
        PatientVisit visit = null;
        for(int i = 0; i < visits.Count; i++)
        {
            if(visits[i].Id == id)
            {
                visit = visits[i];
                break;
            }
        }
        
        if (visit == null)
        {
            Console.WriteLine("Visit not found bro!");
            return;
        }

        if (patientName != null) visit.PatientName = patientName;
        if (doctorName != null) visit.DoctorName = doctorName;
        if (visitDate.HasValue) visit.VisitDate = visitDate.Value;
        if (visitType != null) visit.VisitType = visitType;
        if (notes != null) visit.Notes = notes;

        saveData();
        Console.WriteLine("Updated successfully!");
    }

    public void DeleteVisit(int id)
    {
        PatientVisit visitToRemove = null;
        for(int i = 0; i < visits.Count; i++)
        {
            if(visits[i].Id == id)
            {
                visitToRemove = visits[i];
                break;
            }
        }
        
        if (visitToRemove == null)
        {
            Console.WriteLine("Visit not found!");
            return;
        }

        visits.Remove(visitToRemove);
        saveData();
        Console.WriteLine("Visit deleted!");
    }

    public PatientVisit findById(int id)
    {
        for(int i = 0; i < visits.Count; i++)
        {
            if(visits[i].Id == id)
                return visits[i];
        }
        return null;
    }

    public List<PatientVisit> getAllVisits()
    {
        return visits;
    }

    public void showVisit(PatientVisit visit)
    {
        if (visit == null)
        {
            Console.WriteLine("No visit found man!");
            return;
        }

        Console.WriteLine("\n" + "=".PadRight(80, '='));
        Console.WriteLine("ID: " + visit.Id);
        Console.WriteLine("Patient: " + visit.PatientName);
        Console.WriteLine("Doctor: " + visit.DoctorName);
        Console.WriteLine("Date: " + visit.VisitDate.ToString("yyyy-MM-dd"));
        Console.WriteLine("Type: " + visit.VisitType);
        Console.WriteLine("Notes: " + visit.Notes);
        Console.WriteLine("=".PadRight(80, '='));
    }

    public void showAllRecords()
    {
        if (visits.Count == 0)
        {
            Console.WriteLine("No records found buddy!");
            return;
        }

        Console.WriteLine("\n--- All Patient Records ---");
        for(int i = 0; i < visits.Count; i++)
        {
            PatientVisit v = visits[i];
            Console.WriteLine($"{v.Id} | {v.PatientName} | {v.DoctorName} | {v.VisitDate:yyyy-MM-dd} | {v.VisitType} | {v.Notes}");
        }
        Console.WriteLine("--- End of Records ---");
    }

    private void saveData()
    {
        try
        {
            StreamWriter writer = new StreamWriter(filePath);
            for(int i = 0; i < visits.Count; i++)
            {
                PatientVisit visit = visits[i];
                string line = visit.Id + "|" + visit.PatientName + "|" + visit.DoctorName + "|" + 
                             visit.VisitDate.ToString("yyyy-MM-dd HH:mm:ss") + "|" + visit.VisitType + "|" + visit.Notes;
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
                for(int i = 0; i < lines.Length; i++)
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
                            visit.DoctorName = parts[2];
                            visit.VisitDate = DateTime.Parse(parts[3]);
                            visit.VisitType = parts[4];
                            visit.Notes = parts.Length > 5 ? parts[5] : "";
                            
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
    public static void Main()
    {
        PatientVisitManager manager = new PatientVisitManager();
        
        while (true)
        {
            Console.WriteLine("\n*** Patient Visit System ***");
            Console.WriteLine("1. Add New Visit");
            Console.WriteLine("2. Update Visit");
            Console.WriteLine("3. Delete Visit");
            Console.WriteLine("4. Search by ID");
            Console.WriteLine("5. Show All Records");
            Console.WriteLine("6. Exit");
            Console.Write("Choose option: ");
            
            string input = Console.ReadLine();
            
            if(input == "1")
            {
                addNewVisit(manager);
            }
            else if(input == "2")
            {
                updateVisit(manager);
            }
            else if(input == "3")
            {
                deleteVisit(manager);
            }
            else if(input == "4")
            {
                searchById(manager);
            }
            else if(input == "5")
            {
                manager.showAllRecords();
            }
            else if(input == "6")
            {
                Console.WriteLine("Bye!");
                break;
            }
            else
            {
                Console.WriteLine("Wrong choice! Try again.");
            }
        }
    }

    private static void addNewVisit(PatientVisitManager manager)
    {
        Console.Write("Patient Name: ");
        string patientName = Console.ReadLine();
        
        Console.Write("Doctor Name: ");
        string doctorName = Console.ReadLine();
        
        Console.Write("Visit Date (yyyy-mm-dd) or Enter for today: ");
        string dateStr = Console.ReadLine();
        DateTime visitDate;
        if(string.IsNullOrEmpty(dateStr))
            visitDate = DateTime.Now;
        else
            visitDate = DateTime.Parse(dateStr);
        
        Console.Write("Visit Type: ");
        string visitType = Console.ReadLine();
        
        Console.Write("Notes: ");
        string notes = Console.ReadLine();
        
        manager.AddVisit(patientName, doctorName, visitDate, visitType, notes);
    }

    private static void updateVisit(PatientVisitManager manager)
    {
        Console.Write("Enter Visit ID: ");
        int id = int.Parse(Console.ReadLine());
        
        Console.Write("New Patient Name (Enter to skip): ");
        string patientName = Console.ReadLine();
        
        Console.Write("New Doctor Name (Enter to skip): ");
        string doctorName = Console.ReadLine();
        
        Console.Write("New Date (yyyy-mm-dd) (Enter to skip): ");
        string dateStr = Console.ReadLine();
        DateTime? visitDate = null;
        if(!string.IsNullOrEmpty(dateStr))
            visitDate = DateTime.Parse(dateStr);
        
        Console.Write("New Visit Type (Enter to skip): ");
        string visitType = Console.ReadLine();
        
        Console.Write("New Notes (Enter to skip): ");
        string notes = Console.ReadLine();
        
        manager.UpdateVisit(id, 
            string.IsNullOrEmpty(patientName) ? null : patientName,
            string.IsNullOrEmpty(doctorName) ? null : doctorName,
            visitDate,
            string.IsNullOrEmpty(visitType) ? null : visitType,
            string.IsNullOrEmpty(notes) ? null : notes);
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
        PatientVisit result = manager.findById(id);
        manager.showVisit(result);
    }
}