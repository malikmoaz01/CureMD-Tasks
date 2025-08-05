using System;

namespace ECommerceOrderSystem.Singletons
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogWarning(string message);
    }

    public sealed class Logger : ILogger
    {
        private static readonly object _lock = new object();
        private static Logger _instance = null;

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Logger();
                    }
                }
                return _instance;
            }
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        private void WriteLog(string level, string message)
        {
            lock (_lock)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}");
            }
        }
    }
}
