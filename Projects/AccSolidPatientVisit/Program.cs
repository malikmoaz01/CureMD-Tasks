using System;

// SRP: Single Responsibility - Application startup and coordination
// DIP: Depends on abstractions through dependency injection
public class Program
{
    private static UserRole _currentUserRole;
    private static IAdminService _adminService;
    private static IReceptionistService _receptionistService;
    private static MenuService _menuService;
    private static AuthenticationService _authService;
    private static INotificationService _notificationService;

    public static void Main()
    {
        // DIP: Dependency Injection - Create all dependencies
        InitializeDependencies();

        // Authenticate user
        var (success, role) = _authService.AuthenticateUser();
        if (!success)
        {
            Console.WriteLine("Authentication failed. Exiting...");
            return;
        }

        _currentUserRole = role;

        // Main application loop
        RunApplication();
    }

    // DIP: Manual dependency injection setup
    private static void InitializeDependencies()
    {
        // Create concrete implementations
        IVisitRepository repository = new FileVisitRepository();
        ILogger logger = new ActivityLogger();
        _notificationService = new ConsoleNotificationService();
        IFeeCalculator feeCalculator = new FeeCalculator();
        IDisplayService displayService = new ConsoleDisplayService();

        // Create core services
        IVisitManager visitManager = new VisitManager(repository, logger, _notificationService, feeCalculator);
        CommandManager commandManager = new CommandManager(_notificationService);
        VisitService visitService = new VisitService(visitManager, commandManager, displayService, _notificationService, logger);

        // Create role-based services
        _adminService = new AdminService(visitService, visitManager, _notificationService, logger);
        _receptionistService = new ReceptionistService(visitService);

        // Create UI services
        _menuService = new MenuService(_notificationService);
        _authService = new AuthenticationService(_notificationService);
    }

    private static void RunApplication()
    {
        while (true)
        {
            _menuService.ShowMainMenu(_currentUserRole);
            string input = Console.ReadLine();

            try
            {
                HandleMenuChoice(input);
            }
            catch (Exception ex)
            {
                _notificationService.ShowError("An error occurred: " + ex.Message);
            }
        }
    }

    private static void HandleMenuChoice(string input)
    {
        switch (input)
        {
            case "1":
                HandleAddNewVisit();
                break;

            case "2" when _currentUserRole == UserRole.Admin:
                HandleUpdateVisit();
                break;

            case "3" when _currentUserRole == UserRole.Admin:
                HandleDeleteVisit();
                break;

            case "4":
                HandleSearchById();
                break;

            case "5":
                if (_currentUserRole == UserRole.Admin)
                    _adminService.ShowAllRecords();
                else
                    _receptionistService.ShowAllRecords();
                break;

            case "6":
                if (_currentUserRole == UserRole.Admin)
                    _adminService.ShowVisitsByType();
                else
                    _receptionistService.ShowVisitsByType();
                break;

            case "7":
                HandleGenerateIndividualSummary();
                break;

            case "8":
                if (_currentUserRole == UserRole.Admin)
                    _adminService.GetTotalVisitsByType();
                else
                    _receptionistService.GetTotalVisitsByType();
                break;

            case "9":
                if (_currentUserRole == UserRole.Admin)
                    _adminService.GetWeeklySummary();
                else
                    _receptionistService.GetWeeklySummary();
                break;

            case "10" when _currentUserRole == UserRole.Admin:
                HandleGenerateMockData();
                break;

            case "11":
                HandleFilterAndSortMenu();
                break;

            case "12":
                Environment.Exit(0);
                break;

            case "13" when _currentUserRole == UserRole.Admin:
                _adminService.UndoLastAction();
                break;

            case "14" when _currentUserRole == UserRole.Admin:
                _adminService.RedoLastAction();
                break;

            default:
                _notificationService.ShowError("Wrong choice! Try again.");
                break;
        }
    }

    private static void HandleAddNewVisit()
    {
        var (patientName, visitDate, visitType, note, doctorName, duration) = _menuService.GetNewVisitInput();

        if (_currentUserRole == UserRole.Admin)
            _adminService.AddVisit(patientName, visitDate, visitType, note, doctorName, duration);
        else
            _receptionistService.AddVisit(patientName, visitDate, visitType, note, doctorName, duration);
    }

    private static void HandleUpdateVisit()
    {
        var (id, patientName, visitDate, visitType, note, doctorName, duration) = _menuService.GetUpdateVisitInput();
        _adminService.UpdateVisit(id, patientName, visitDate, visitType, note, doctorName, duration);
    }

    private static void HandleDeleteVisit()
    {
        int id = _menuService.GetVisitId("delete");
        _adminService.DeleteVisit(id);
    }

    private static void HandleSearchById()
    {
        int id = _menuService.GetVisitId("search");

        if (_currentUserRole == UserRole.Admin)
            _adminService.ShowVisit(id);
        else
            _receptionistService.ShowVisit(id);
    }

    private static void HandleGenerateIndividualSummary()
    {
        int id = _menuService.GetVisitId("generate summary for");

        if (_currentUserRole == UserRole.Admin)
            _adminService.GenerateVisitSummary(id);
        else
            _receptionistService.GenerateVisitSummary(id);
    }

    private static void HandleGenerateMockData()
    {
        int count = _menuService.GetMockDataCount();
        _adminService.GenerateMockData(count);
    }

    private static void HandleFilterAndSortMenu()
    {
        _menuService.ShowFilterAndSortMenu();
        string choice = Console.ReadLine();

        if (choice == "1" || choice == "2")
        {
            var (filterType, filterValue) = _menuService.GetFilterInput(choice);
            if (_currentUserRole == UserRole.Admin)
                _adminService.ShowFilteredRecords(filterType, filterValue);
            else
                _receptionistService.ShowFilteredRecords(filterType, filterValue);
        }
        else if (choice == "3")
        {
            var (startDate, endDate) = _menuService.GetDateRangeInput();
            if (_currentUserRole == UserRole.Admin)
                _adminService.ShowFilteredRecords(startDate, endDate);
            else
                _receptionistService.ShowFilteredRecords(startDate, endDate);
        }
        else if (choice == "4" || choice == "5" || choice == "6" || choice == "7")
        {
            string sortBy = _menuService.GetSortCriteria(choice);
            if (_currentUserRole == UserRole.Admin)
                _adminService.ShowSortedRecords(sortBy);
            else
                _receptionistService.ShowSortedRecords(sortBy);
        }
    }
}