﻿using SupernoteDesktopClient.Core.Win32Api;
using System.Collections.Generic;

namespace SupernoteDesktopClient.Models
{
    public class Settings
    {
        public int LatestVersion { get { return 1; } }
        public int CurrentVersion { get; set; } = 1;
        public Dictionary<string, SupernoteInfo> DeviceProfiles { get; set; } = new Dictionary<string, SupernoteInfo>();

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

    public class General
    {
        public bool RememberAppWindowPlacement { get; set; } = true;
        public WindowPlacement AppWindowPlacement { get; set; }
        public bool MinimizeToTrayEnabled { get; set; } = false;
        public string CurrentTheme { get; set; } = "Light"; // Light or Dark
        public bool DiagnosticLogEnabled { get; set; } = false;
        public bool AutomaticUpdateCheckEnabled { get; set; } = true;
    }

    public class Sync
    {
        public bool ShowNotificationOnDeviceStateChange { get; set; } = true;
        public bool AutomaticSyncOnConnect { get; set; } = false;
        public int MaxDeviceArchives { get; set; } = 7;
    }
}
