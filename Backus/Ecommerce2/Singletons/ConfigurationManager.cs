using System.Collections.Generic;

namespace ECommerce2.Singletons
{
    public interface IConfigurationManager
    {
        string GetSetting(string key);
        void SetSetting(string key, string value);
    }

    public sealed class ConfigurationManager : IConfigurationManager
    {
        private static ConfigurationManager instance = null;
        private static readonly object lockObject = new object();
        private Dictionary<string, string> settings;

        private ConfigurationManager()
        {
            settings = new Dictionary<string, string>();
            InitializeDefaultSettings();
        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                            instance = new ConfigurationManager();
                    }
                }
                return instance;
            }
        }

        private void InitializeDefaultSettings()
        {
            settings["DatabaseConnection"] = "localhost:1433";
            settings["APIKey"] = "default-api-key";
            settings["MaxOrderItems"] = "50";
            settings["TaxRate"] = "0.08";
        }

        public string GetSetting(string key)
        {
            lock (lockObject)
            {
                return settings.ContainsKey(key) ? settings[key] : null;
            }
        }

        public void SetSetting(string key, string value)
        {
            lock (lockObject)
            {
                settings[key] = value;
            }
        }
    }
}