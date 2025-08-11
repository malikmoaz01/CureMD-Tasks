using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
 
public class FeeCalculator : IFeeCalculator
{
    private Dictionary<string, decimal> _feeRates;
    private readonly string _feeFilePath;

    public FeeCalculator(string feeFilePath = "fees.json")
    {
        _feeFilePath = feeFilePath;
        LoadFeeRates();
    }

    public void LoadFeeRates()
    {
        try
        {
            if (File.Exists(_feeFilePath))
            {
                string json = File.ReadAllText(_feeFilePath);
                _feeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
            }
            else
            {
                _feeRates = new Dictionary<string, decimal>
                {
                    {"Consultation", 500},
                    {"Follow-up", 300},
                    {"Emergency", 1000}
                };
                SaveFeeRates();
            }
        }
        catch (Exception)
        {
            _feeRates = new Dictionary<string, decimal>
            {
                {"Consultation", 500},
                {"Follow-up", 300},
                {"Emergency", 1000}
            };
        }
    }

    public void SaveFeeRates()
    {
        try
        {
            string json = JsonSerializer.Serialize(_feeRates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_feeFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving fee rates: " + ex.Message);
        }
    }

    public decimal CalculateFee(string visitType, int duration)
    {
        if (_feeRates.ContainsKey(visitType))
        {
            decimal baseRate = _feeRates[visitType];
            if (duration > 30)
            {
                decimal extraTime = (duration - 30) / 15.0m;
                return baseRate + (extraTime * (baseRate * 0.25m));
            }
            return baseRate;
        }
        return 500;
    }
}