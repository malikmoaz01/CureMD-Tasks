using System;
using System.IO;

// SRP: Single Responsibility - Logging only
// OCP: Open for extension (can create different loggers)
// DIP: Implements abstraction
public class ActivityLogger : ILogger
{
    private readonly string _logFilePath;

    public ActivityLogger(string logFilePath = "activity_log.txt")
    {
        _logFilePath = logFilePath;
    }

    public void LogActivity(string action, bool success, string details = "")
    {
        try
        {
            string logEntry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + action + " | " + (success ? "SUCCESS" : "FAILURE") + " | " + details;
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error logging activity: " + ex.Message);
        }
    }
}