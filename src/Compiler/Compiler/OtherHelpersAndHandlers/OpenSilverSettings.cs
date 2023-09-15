using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSilver.Compiler
{
    internal class OpenSilverSettings
    {
        private const string AppName = "OpenSilver";

        private static string _settingsPath = GetSettingsPath();

        private static OpenSilverSettings? _instance;

        private Dictionary<string, string> _settingsDictionary;
        Dictionary<string, string> SettingsDictionary => _settingsDictionary ??= InitializeSettings();

        #region initializers
        void InitializeFile()
        {
            if (!System.IO.File.Exists(_settingsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath));
                File.Create(_settingsPath);
            }
        }

        Dictionary<string, string> InitializeSettings()
        {
            InitializeFile();

            //read the settings from the file if it already existed
            using (FileStream settingsStream = new FileStream(_settingsPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                using (StreamReader reader = new StreamReader(settingsStream))
                {
                    string fileContents = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(fileContents))
                    {
                        _settingsDictionary = new Dictionary<string, string>();
                    }
                    else
                    {
                        _settingsDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(fileContents);
                    }
                }
            }
            return _settingsDictionary;
        }


        private static string GetSettingsPath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fullPath = Path.Combine(appDataFolder, AppName, "app.json");
            //Console.WriteLine(fullPath);
            return fullPath;
        }
        #endregion

        public static OpenSilverSettings Instance
        {
            get
            {
                _instance ??= new OpenSilverSettings();
                return _instance;
            }
        }

        public void SaveSettings()
        {
            using (FileStream settingsStream = new FileStream(_settingsPath, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(settingsStream))
                {
                    string newContents = System.Text.Json.JsonSerializer.Serialize(SettingsDictionary);
                    writer.Write(newContents);
                }
            }
        }

        public void SetValue(string name, string value)
        {
            SettingsDictionary[name] = value;
        }

        public string? GetValue(string name)
        {
            if (SettingsDictionary.ContainsKey(name))
            {
                return SettingsDictionary[name];
            }

            return null;
        }

    }
}
