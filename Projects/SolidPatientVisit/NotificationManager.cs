using System;

public class NotificationManager
{
    public bool ShowTimeConflictWarning(string patientName)
    {
        Console.WriteLine("Warning: This patient has another visit within 30 minutes. Proceed? (Y/N)");
        string response = Console.ReadLine();
        return response.ToUpper() == "Y";
    }

    public void ShowVisitCancelledDueToConflict()
    {
        Console.WriteLine("Visit cancelled due to time conflict.");
    }

    public void ShowVisitAdded(int id, string patientName)
    {
        Console.WriteLine("Visit Added -> Visit id & Name for this visit are  " + id + " -> " + patientName);
    }

    public void ShowVisitNotFound()
    {
        Console.WriteLine("Visit not found bro!");
    }

    public void ShowUpdateSuccessful()
    {
        Console.WriteLine("Updated successfully!");
    }

    public void ShowVisitDeleted()
    {
        Console.WriteLine("Visit deleted.");
    }

    public void ShowUndoComplete()
    {
        Console.WriteLine("Undo completed.");
    }

    public void ShowNoActionsToUndo()
    {
        Console.WriteLine("No actions to undo.");
    }

    public void ShowRedoComplete()
    {
        Console.WriteLine("Redo completed.");
    }

    public void ShowNoActionsToRedo()
    {
        Console.WriteLine("No actions to redo.");
    }

    public void ShowGeneratingMockData(int count)
    {
        Console.WriteLine("Generating " + count + " mock entries...");
    }

    public void ShowMockDataGenerated(int count, int totalRecords)
    {
        Console.WriteLine("Mock data generated successfully! Total records: " + totalRecords);
    }

    public void ShowNoRecordsFound()
    {
        Console.WriteLine("No records found");
    }

    public void ShowNoVisitFound()
    {
        Console.WriteLine("No visit found ");
    }

    public void ShowNoRecordsMatchingCriteria()
    {
        Console.WriteLine("No records found matching criteria");
    }

    public void ShowVisitNotFoundForSummary()
    {
        Console.WriteLine("Visit not found!");
    }

    public void ShowWrongChoice()
    {
        Console.WriteLine("Wrong choice! Try again.");
    }

    public void ShowAuthenticationFailed()
    {
        Console.WriteLine("Authentication failed. Exiting...");
    }

    public void ShowAdminLoginSuccessful()
    {
        Console.WriteLine("Admin login successful!");
    }

    public void ShowReceptionistLoginSuccessful()
    {
        Console.WriteLine("Receptionist login successful!");
    }
}