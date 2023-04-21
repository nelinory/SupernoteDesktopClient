﻿using SupernoteDesktopClient.Core.Win32Api;

namespace SupernoteDesktopClient.Models
{
    public class Settings
    {
        public int LatestVersion { get { return 1; } }
        public int CurrentVersion { get; set; } = 1;

        public General General { get; set; }
        public Sync Sync { get; set; }
        
        public Settings()
        {
            CurrentVersion = LatestVersion;

            // sections
            General = new General();
            Sync = new Sync();
        }
    }

    // TODO: Add to Settings View
    public class General
    {
        public bool RememberAppWindowPlacement { get; set; } = true;
        public WindowPlacement AppWindowPlacement { get; set; }

        public string CurrentTheme { get; set; } = "Light"; // Light or Dark
    }

    // TODO: Add to Settings View
    public class Sync
    {
        public bool ShowNotificationOnDeviceStateChange { get; set; } = true;
        public int MaxDeviceBackups { get; set; } = 5;
    }
}