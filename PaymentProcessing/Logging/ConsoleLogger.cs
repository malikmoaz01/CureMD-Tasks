using System;
using PaymentProcessing.Abstractions;

namespace PaymentProcessing.Logging
{
    public class ConsoleLogger : ILoggerService
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG]: {message}");
        }
    }
}