using System;
using System.Collections.Generic;
using System.IO;
 
public class FileVisitRepository : IVisitRepository
{
    private readonly string _filePath;

    public FileVisitRepository(string filePath = "patient_visits.txt")
    {
        _filePath = filePath;
    }

    public void SaveVisits(List<PatientVisit> visits)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                foreach (var visit in visits)
                {
                    string line = $"{visit.Id}|{visit.PatientName}|{visit.VisitDate:yyyy-MM-dd HH:mm:ss}|{visit.VisitType}|{visit.Note}|{visit.DoctorName}|{visit.DurationInMinutes}|{visit.Fee}";
                    writer.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }

    public List<PatientVisit> LoadVisits()
    {
        var visits = new List<PatientVisit>();
        try
        {
            if (File.Exists(_filePath))
            {
                string[] lines = File.ReadAllLines(_filePath);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 5)
                        {
                            var visit = new PatientVisit
                            {
                                Id = int.Parse(parts[0]),
                                PatientName = parts[1],
                                VisitDate = DateTime.Parse(parts[2]),
                                VisitType = parts[3],
                                Note = parts[4],
                                DoctorName = parts.Length > 5 ? parts[5] : "",
                                DurationInMinutes = parts.Length > 6 ? int.Parse(parts[6]) : 30,
                                Fee = parts.Length > 7 ? decimal.Parse(parts[7]) : 0
                            };
                            visits.Add(visit);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
        }
        return visits;
    }
}