using System;
using System.Collections.Generic;
using System.Linq;

public class ReportManager
{
    private NotificationManager notificationManager;

    public ReportManager(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
    }

    public void showVisit(PatientVisit visit)
    {
        if (visit == null)
        {
            notificationManager.ShowNoVisitFound();
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

    public void showAllRecords(List<PatientVisit> visits)
    {
        if (visits.Count == 0)
        {
            notificationManager.ShowNoRecordsFound();
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
            notificationManager.ShowNoRecordsMatchingCriteria();
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

    public void showvisitsbytype(List<PatientVisit> visits)
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

    public void generateVisitSummary(PatientVisit visit)
    {
        if (visit == null)
        {
            notificationManager.ShowVisitNotFoundForSummary();
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

    public void getTotalVisitsByType(List<PatientVisit> visits)
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

    public void getWeeklySummary(List<PatientVisit> visits)
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
}