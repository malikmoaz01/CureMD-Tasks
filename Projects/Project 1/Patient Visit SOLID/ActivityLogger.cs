using System;
using System.IO;
 
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