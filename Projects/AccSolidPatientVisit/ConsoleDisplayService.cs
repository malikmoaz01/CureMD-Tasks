using System;
using System.Collections.Generic;
using System.Linq;

// SRP: Single Responsibility - Display operations only
// OCP: Open for extension (can create different display services)
// DIP: Implements abstraction
public class ConsoleDisplayService : IDisplayService
{
    public void ShowVisit(PatientVisit visit)
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

    public void ShowAllRecords(List<PatientVisit> visits)
    {
        if (visits.Count == 0)
        {
            Console.WriteLine("No records found");
            return;
        }

        Console.WriteLine("\n ===== Patients Records ======\n");

        foreach (var v in visits)
        {
            Console.WriteLine($"{v.Id} | {v.PatientName} | {v.VisitDate:yyyy-MM-dd HH:mm:ss} | {v.VisitType} | {v.Note} | {v.DoctorName} | {v.DurationInMinutes}min | Rs.{v.Fee}");
        }

        Console.WriteLine("\n===============================\n");
    }

    public void ShowFilteredAndSortedRecords(List<PatientVisit> visitList)
    {
        if (visitList.Count == 0)
        {
            Console.WriteLine("No records found matching criteria");
            return;
        }

        Console.WriteLine("\n ===== Filtered/Sorted Records ======\n");

        foreach (var v in visitList)
        {
            Console.WriteLine($"{v.Id} | {v.PatientName} | {v.VisitDate:yyyy-MM-dd} | {v.VisitType} | {v.Note} | {v.DoctorName} | {v.DurationInMinutes}min | Rs.{v.Fee}");
        }

        Console.WriteLine("\n===============================\n");
    }

    public void ShowVisitsByType(List<PatientVisit> visits)
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();

        foreach (var visit in visits)
        {
            string type = visit.VisitType;
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

    public void GenerateVisitSummary(PatientVisit visit)
    {
        if (visit == null)
        {
            Console.WriteLine("Visit not found!");
            return;
        }

        Console.WriteLine("\n=== Individual Visit Summary ===");
        Console.WriteLine("Visit ID: " + visit.Id);
        Console.WriteLine("Patient Name: " + visit.PatientName);
        Console.WriteLine("Visit Date: " + visit.VisitDate.ToString("dddd, MMMM dd, yyyy"));
        Console.WriteLine("Visit Type: " + visit.VisitType);
        Console.WriteLine("Notes: " + visit.Note);
        Console.WriteLine("Attending Doctor: " + (string.IsNullOrEmpty(visit.DoctorName) ? "Not assigned" : visit.DoctorName));
        Console.WriteLine("Duration: " + visit.DurationInMinutes + " minutes");
        Console.WriteLine("Fee: Rs. " + visit.Fee);
        Console.WriteLine("\n=================================\n");
    }

    public void GetTotalVisitsByType(List<PatientVisit> visits)
    {
        Dictionary<string, int> dict_count = new Dictionary<string, int>();

        foreach (var visit in visits)
        {
            string type = visit.VisitType;
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

    public void GetWeeklySummary(List<PatientVisit> visits)
    {
        DateTime today = DateTime.Now;
        DateTime weekStart = today.AddDays(-(int)today.DayOfWeek);
        DateTime weekEnd = weekStart.AddDays(7);

        List<PatientVisit> weeklyVisits = visits.Where(v => v.VisitDate >= weekStart && v.VisitDate < weekEnd).ToList();

        Console.WriteLine("\n*** Weekly Visit Summary ***");
        Console.WriteLine("Week: " + weekStart.ToString("MMM dd") + " - " + weekEnd.AddDays(-1).ToString("MMM dd, yyyy"));
        Console.WriteLine("Total visits this week: " + weeklyVisits.Count);

        Dictionary<string, int> dailyCounts = new Dictionary<string, int>();

        foreach (var visit in weeklyVisits)
        {
            string day = visit.VisitDate.ToString("dddd");
            if (dailyCounts.ContainsKey(day))
                dailyCounts[day]++;
            else
                dailyCounts[day] = 1;
        }

        Console.WriteLine("\nDaily breakdown:");

        string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        foreach (string day in days)
        {
            int count = dailyCounts.ContainsKey(day) ? dailyCounts[day] : 0;
            Console.WriteLine(day + ": " + count + " visits");
        }

        Console.WriteLine("****************************");
    }
}