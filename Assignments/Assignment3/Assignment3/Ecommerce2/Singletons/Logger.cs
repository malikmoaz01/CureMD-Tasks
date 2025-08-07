using System;

namespace ECommerce2.Singletons
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogWarning(string message);
    }

    public sealed class Logger : ILogger
    {
        private static Logger instance = null;
        private static readonly object lockObject = new object();

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new Logger();
                    }
                }
                return instance;
            }
        }

        public void LogInfo(string message)
        {
            lock (lockObject)
            {
                Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }

        public void LogError(string message)
        {
            lock (lockObject)
            {
                Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }

        public void LogWarning(string message)
        {
            lock (lockObject)
            {
                Console.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }
    }
}
