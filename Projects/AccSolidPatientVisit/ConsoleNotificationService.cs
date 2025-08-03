using System;

// SRP: Single Responsibility - User notifications only
// OCP: Open for extension (can create UI notifications)
// DIP: Implements abstraction
public class ConsoleNotificationService : INotificationService
{
    public void ShowSuccess(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowError(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowWarning(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowInfo(string message)
    {
        Console.WriteLine(message);
    }

    public bool GetConfirmation(string message)
    {
        Console.WriteLine(message);
        string response = Console.ReadLine();
        return response?.ToUpper() == "Y";
    }
}