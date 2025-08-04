using System;
 
public class AuthenticationService
{
    private readonly INotificationService _notificationService;

    public AuthenticationService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public (bool success, UserRole role) AuthenticateUser()
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
            _notificationService.ShowSuccess("Admin login successful!");
            return (true, UserRole.Admin);
        }
        else if (roleChoice == "2" && username == "curemd1" && password == "curemd1")
        {
            _notificationService.ShowSuccess("Receptionist login successful!");
            return (true, UserRole.Receptionist);
        }

        _notificationService.ShowError("Authentication failed. Invalid credentials.");
        return (false, UserRole.Admin);
    }
}