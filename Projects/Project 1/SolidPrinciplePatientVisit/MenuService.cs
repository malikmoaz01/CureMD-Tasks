using System;
 
public class MenuService
{
    private readonly INotificationService _notificationService;

    public MenuService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void ShowMainMenu(UserRole currentUserRole)
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

    public void ShowFilterAndSortMenu()
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
    }

    public (string patientName, DateTime visitDate, string visitType, string note, string doctorName, int duration) GetNewVisitInput()
    {
        Console.Write("Patient Name: ");
        string patientName = Console.ReadLine();

        Console.Write("Visit Date (yyyy-mm-dd) or Enter for today: ");
        string dateStr = Console.ReadLine();
        DateTime visitDate = string.IsNullOrEmpty(dateStr) ? DateTime.Now : DateTime.Parse(dateStr);

        Console.Write("Visit Type: ");
        string visitType = Console.ReadLine();

        Console.Write("Notes: ");
        string note = Console.ReadLine();

        Console.Write("Doctor Name (optional): ");
        string doctorName = Console.ReadLine();

        Console.Write("Duration in minutes (default 30): ");
        string durationStr = Console.ReadLine();
        int duration = string.IsNullOrEmpty(durationStr) ? 30 : int.Parse(durationStr);

        return (patientName, visitDate, visitType, note, doctorName, duration);
    }

    public (int id, string patientName, DateTime? visitDate, string visitType, string note, string doctorName, int? duration) GetUpdateVisitInput()
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

        return (id, patientName, visitDate, visitType, note, doctorName, duration);
    }

    public int GetVisitId(string operation)
    {
        Console.Write($"Enter Visit ID to {operation}: ");
        return int.Parse(Console.ReadLine());
    }

    public int GetMockDataCount()
    {
        Console.Write("Enter number of mock records to generate (default 350): ");
        string countStr = Console.ReadLine();
        return string.IsNullOrEmpty(countStr) ? 350 : int.Parse(countStr);
    }

    public (string filterType, string filterValue) GetFilterInput(string choice)
    {
        switch (choice)
        {
            case "1":
                Console.Write("Enter doctor name: ");
                return ("doctor", Console.ReadLine());
            case "2":
                Console.Write("Enter visit type: ");
                return ("type", Console.ReadLine());
            default:
                return ("", "");
        }
    }

    public (DateTime startDate, DateTime endDate) GetDateRangeInput()
    {
        Console.Write("Enter start date (yyyy-mm-dd): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter end date (yyyy-mm-dd): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());
        return (startDate, endDate);
    }

    public string GetSortCriteria(string choice)
    {
        switch (choice)
        {
            case "4": return "date";
            case "5": return "name";
            case "6": return "type";
            case "7": return "fee";
            default: return "date";
        }
    }
}