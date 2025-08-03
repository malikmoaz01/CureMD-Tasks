using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class FileManager
{
    private string filePath;
    private string logFilePath = "activity_log.txt";
    private string feeFilePath = "fees.json";

    public FileManager(string fileName = "patient_visits.txt")
    {
        filePath = fileName;
    }

    public void SaveVisits(List<PatientVisit> visits)
    {
        try
        {
            StreamWriter writer = new StreamWriter(filePath);
            for (int i = 0; i < visits.Count; i++)
            {
                PatientVisit visit = visits[i];
                string line = visit.Id + "|" + visit.PatientName + "|" +
                             visit.VisitDate.ToString("yyyy-MM-dd HH:mm:ss") + "|" +
                             visit.VisitType + "|" + visit.Note + "|" + visit.DoctorName + "|" +
                             visit.DurationInMinutes + "|" + visit.Fee;
                writer.WriteLine(line);
            }
            writer.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }

    public (List<PatientVisit> visits, int nextId) LoadVisits()
    {
        List<PatientVisit> visits = new List<PatientVisit>();
        int nextId = 1;

        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length >= 5)
                        {
                            PatientVisit visit = new PatientVisit();
                            visit.Id = int.Parse(parts[0]);
                            visit.PatientName = parts[1];
                            visit.VisitDate = DateTime.Parse(parts[2]);
                            visit.VisitType = parts[3];
                            visit.Note = parts[4];
                            visit.DoctorName = parts.Length > 5 ? parts[5] : "";
                            visit.DurationInMinutes = parts.Length > 6 ? int.Parse(parts[6]) : 30;
                            visit.Fee = parts.Length > 7 ? decimal.Parse(parts[7]) : calculateFee(visit.VisitType, visit.DurationInMinutes);

                            visits.Add(visit);
                            if (visit.Id >= nextId)
                                nextId = visit.Id + 1;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
        }

        return (visits, nextId);
    }

    public Dictionary<string, decimal> LoadFeeRates()
    {
        Dictionary<string, decimal> feeRates;

        try
        {
            if (File.Exists(feeFilePath))
            {
                string json = File.ReadAllText(feeFilePath);
                feeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
            }
            else
            {  
                feeRates = new Dictionary<string, decimal>()
                {
                    {"Consultation", 500},
                    {"Follow-up", 300},
                    {"Emergency", 1000}
                };
                
                SaveFeeRates(feeRates);
            }
        }
        catch (Exception)
        {
            feeRates = new Dictionary<string, decimal>()
            {
                {"Consultation", 500},
                {"Follow-up", 300},
                {"Emergency", 1000}
            };
        }

        return feeRates;
    }

    private void SaveFeeRates(Dictionary<string, decimal> feeRates)
    {
        try
        {
            string json = JsonSerializer.Serialize(feeRates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(feeFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving fee rates: " + ex.Message);
        }
    }

    public void LogActivity(string action, bool success, string details = "")
    {
        try
        {
            string logEntry = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") 
                       + 
                            " | " 
                       +
                             action 
                       + 
                             " | " 
                       + 
                             (success ? "SUCCESS" : "FAILURE") 
                       +
                             " | " 
                       +
                             details;

            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error logging activity: " + ex.Message);
        }
    }

    private decimal calculateFee(string visitType, int duration)
    {
        var feeRates = LoadFeeRates();
        if (feeRates.ContainsKey(visitType))
        {
            decimal baseRate = feeRates[visitType];
            if (duration > 30)
            {
                decimal extraTime = (duration - 30) / 15.0m;
                decimal extraTime2 = baseRate + (extraTime * (baseRate * 0.25m));
                return extraTime2;
            }
            return baseRate;
        }
        return 500;
    }
}