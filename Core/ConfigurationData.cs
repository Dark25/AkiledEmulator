using System;
using System.Collections.Generic;
using System.IO;

namespace Akiled.Core
{
    public class ConfigurationData
    {
        public Dictionary<string, string> data;

        public ConfigurationData(string filePath, bool maynotexist = false)
        {
            this.data = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                if (!maynotexist)
                    throw new ArgumentException("Unable to locate configuration file at '" + filePath + "'.");
            }
            else
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(filePath))
                    {
                        string line;

                        while ((line = streamReader.ReadLine()) != null)
                        {
                            if (line.Length < 1 || line.StartsWith("#")) continue;

                            int delimiterIndex = line.IndexOf('=');
                            if (delimiterIndex == -1) continue;

                            string key = line.Substring(0, delimiterIndex);
                            string val = line.Substring(delimiterIndex + 1);

                            data.Add(key, val);
                        }
                        streamReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Could not process configuration file: " + ex.Message);
                }
            }
        }
    }
}
