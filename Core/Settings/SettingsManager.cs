using Akiled.Database.Interfaces;
using log4net;
using System.Collections.Generic;
using System.Data;

namespace Akiled.Core.Settings
{
    public class SettingsManager
    {
        private readonly Dictionary<string, string> _settings;

        private static readonly ILog Log = LogManager.GetLogger(typeof(SettingsManager));

        public SettingsManager()
        {
            _settings = new Dictionary<string, string>();
        }

        public void Init()
        {
            if (_settings.Count > 0)
                _settings.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_settings`");
                DataTable table = dbClient.GetTable();

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        _settings.Add(row["variable"].ToString().ToLower(), row["value"].ToString().ToLower());
                    }
                }
            }

            Log.Info("Loaded " + _settings.Count + " server settings.");
        }

        public string TryGetValue(string value) => _settings.ContainsKey(value) ? _settings[value] : "0";
    }
}