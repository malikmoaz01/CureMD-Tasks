using System;
using System.Collections.Concurrent;

namespace ECommerceOrderSystem.Singletons
{
    public interface IConfigurationManager
    {
        string GetSetting(string key);
        void SetSetting(string key, string value);
    }

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private static readonly object _lock = new object();
        private static ConfigurationManager _instance = null;
        private readonly ConcurrentDictionary<string, string> _settings;

        private ConfigurationManager()
        {
            _settings = new ConcurrentDictionary<string, string>();
            InitializeDefaultSettings();
        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new ConfigurationManager();
                    }
                }
                return _instance;
            }
        }

        private void InitializeDefaultSettings()
        {
            _settings["DatabaseConnection"] = "Server=localhost;Database=ECommerce;";
            _settings["ApiKey"] = "your-api-key-here";
            _settings["MaxOrderItems"] = "10";
            _settings["Currency"] = "USD";
        }

        public string GetSetting(string key)
        {
            return _settings.TryGetValue(key, out string value) ? value : null;
        }

        public void SetSetting(string key, string value)
        {
            _settings.AddOrUpdate(key, value, (k, v) => value);
        }
    }
}
