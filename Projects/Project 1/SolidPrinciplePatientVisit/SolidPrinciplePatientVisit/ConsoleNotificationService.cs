using System;
 
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