using System;
using System.Collections.Generic;

public class Program
{
    private static UserRole currentUserRole;
    private static VisitManager visitManager;
    private static ReportManager reportManager;
    private static NotificationManager notificationManager;

    public static void Main()
    {
        notificationManager = new NotificationManager();
        
        if (!authenticateUser())
        {
            notificationManager.ShowAuthenticationFailed();
            return;
        }

        FileManager fileManager = new FileManager();
        visitManager = new VisitManager(fileManager, notificationManager);
        reportManager = new ReportManager(notificationManager);

        while (true)
        {
            showMenu();
            string input = Console.ReadLine();

            if (input == "1")
            {
                addNewVisit(visitManager);
            }
            else if (input == "2" && currentUserRole == UserRole.Admin)
            {
                updateVisit(visitManager);
            }
            else if (input == "3" && currentUserRole == UserRole.Admin)
            {
                deleteVisit(visitManager);
            }
            else if (input == "4")
            {
                searchById(visitManager);
            }
            else if (input == "5")
            {
                reportManager.showAllRecords(visitManager.getAllVisits());
            }
            else if (input == "6")
            {
                reportManager.showvisitsbytype(visitManager.getAllVisits());
            }
            else if (input == "7")
            {
                generateIndividualSummary(visitManager);
            }
            else if (input == "8")
            {
                reportManager.getTotalVisitsByType(visitManager.getAllVisits());
            }
            else if (input == "9")
            {
                reportManager.getWeeklySummary(visitManager.getAllVisits());
            }
            else if (input == "10" && currentUserRole == UserRole.Admin)
            {
                generateMockData(visitManager);
            }
            else if (input == "11")
            {
                filterAndSortMenu(visitManager);
            }
            else if (input == "12")
            {
                Environment.Exit(0);
            }
            else if (input == "13" && currentUserRole == UserRole.Admin)
            {
                visitManager.UndoLastAction();
            }
            else if (input == "14" && currentUserRole == UserRole.Admin)
            {
                visitManager.RedoLastAction();
            }
            else
            {
                notificationManager.ShowWrongChoice();
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
            notificationManager.ShowAdminLoginSuccessful();
            return true;
        }
        else if (roleChoice == "2" && username == "curemd1" && password == "curemd1")
        {
            currentUserRole = UserRole.Receptionist;
            notificationManager.ShowReceptionistLoginSuccessful();
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

    private static void filterAndSortMenu(VisitManager manager)
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

        reportManager.showFilteredAndSortedRecords(filteredVisits);
    }

    private static void addNewVisit(VisitManager obj)
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

    private static void updateVisit(VisitManager manager)
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

    private static void deleteVisit(VisitManager manager)
    {
        Console.Write("Enter Visit ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        manager.DeleteVisit(id);
    }

    private static void searchById(VisitManager manager)
    {
        Console.Write("Enter Visit ID: ");

        int id = int.Parse(Console.ReadLine());

        PatientVisit visit = manager.findById(id);

        reportManager.showVisit(visit);
    }

    private static void generateIndividualSummary(VisitManager manager)
    {
        Console.Write("Enter Visit ID for summary: ");

        int id = int.Parse(Console.ReadLine());

        PatientVisit visit = manager.findById(id);
        reportManager.generateVisitSummary(visit);
    }

    private static void generateMockData(VisitManager manager)
    {
        Console.Write("Enter number of mock records to generate (default 350): ");

        string countStr = Console.ReadLine();

        int count = string.IsNullOrEmpty(countStr) ? 350 : int.Parse(countStr);

        manager.generateMockData(count);
    }
}