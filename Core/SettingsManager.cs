using Serilog;
using SupernoteDesktopClient.Models;
using System;
using System.IO;
using System.Text.Json;

namespace SupernoteDesktopClient.Core
{
    public class SettingsManager
    {
        private static readonly Lazy<SettingsManager> _instance = new Lazy<SettingsManager>(() => new SettingsManager());
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        private static readonly string _settingsFileLocation = Path.Combine(FileSystemManager.GetApplicationFolder(), "SupernoteDesktopClient.config");

        public static SettingsManager Instance { get { return _instance.Value; } }

        public Settings Settings { get; private set; }

        private SettingsManager()
        {
            Settings = new Settings();

            Load();
        }

        public void Load()
        {
            if (File.Exists(_settingsFileLocation) == true)
            {
                try
                {
                    Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(_settingsFileLocation));
                }
                catch (Exception ex)
                {
                    Log.Warning("Error while loading settings - will use default: {EX}", ex);
                }
            }
        }

        public void Save()
        {
            string jsonSettings = JsonSerializer.Serialize(Settings, _jsonSerializerOptions);

            File.WriteAllText(_settingsFileLocation, jsonSettings);
        }
    }
}
