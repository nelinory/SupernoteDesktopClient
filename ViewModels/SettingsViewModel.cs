using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Messages;
using System.Collections.Generic;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private const int DEFAULT_MAX_ARCHIVE_DEVICE_INDEX = 2; // 7 archives

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        [ObservableProperty]
        private bool isThemeSwitchChecked = (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark);

        [ObservableProperty]
        private string appThemeName = ApplicationThemeManager.GetAppTheme().ToString();

        public bool RememberAppWindowPlacement
        {
            get { return SettingsManager.Instance.Settings.General.RememberAppWindowPlacement; }
            set { SettingsManager.Instance.Settings.General.RememberAppWindowPlacement = value; }
        }

        public bool MinimizeToTrayEnabled
        {
            get { return SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled; }
            set
            {
                SettingsManager.Instance.Settings.General.MinimizeToTrayEnabled = value;
                NotifySettingsChangedSubscribers(SettingsChangedMessage.MINIMIZE_TO_TRAY_ENABLED);
            }
        }

        public bool AutomaticUpdateCheckEnabled
        {
            get { return SettingsManager.Instance.Settings.General.AutomaticUpdateCheckEnabled; }
            set { SettingsManager.Instance.Settings.General.AutomaticUpdateCheckEnabled = value; }
        }

        public bool DiagnosticLogEnabled
        {
            get { return SettingsManager.Instance.Settings.General.DiagnosticLogEnabled; }
            set { SettingsManager.Instance.Settings.General.DiagnosticLogEnabled = value; }
        }

        public bool ShowNotificationOnDeviceStateChange
        {
            get { return SettingsManager.Instance.Settings.Sync.ShowNotificationOnDeviceStateChange; }
            set { SettingsManager.Instance.Settings.Sync.ShowNotificationOnDeviceStateChange = value; }
        }

        public bool AutomaticSyncOnConnect
        {
            get { return SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect; }
            set { SettingsManager.Instance.Settings.Sync.AutomaticSyncOnConnect = value; }
        }

        public List<int> MaxDeviceArchivesItemSource { get; } = new List<int>() { 1, 5, 7, 10, 15, 20, 25, 30 };

        public int MaxDeviceArchivesSelectedItem
        {
            get
            {
                int index = MaxDeviceArchivesItemSource.FindIndex(p => ((uint)p) == SettingsManager.Instance.Settings.Sync.MaxDeviceArchives);

                return index == -1 ? DEFAULT_MAX_ARCHIVE_DEVICE_INDEX : index;
            }
            set { SettingsManager.Instance.Settings.Sync.MaxDeviceArchives = MaxDeviceArchivesItemSource[value]; }
        }

        public bool StrictModeEnabled
        {
            get { return SettingsManager.Instance.Settings.Conversion.StrictModeEnabled; }
            set { SettingsManager.Instance.Settings.Conversion.StrictModeEnabled = value; }
        }

        public void OnNavigatedTo()
        {
            DiagnosticLogger.Log($"{this}");
        }

        public void OnNavigatedFrom()
        {
        }

        public SettingsViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();

            ApplicationThemeManager.Changed += OnThemeChanged;
        }

        private void OnThemeChanged(ApplicationTheme currentTheme, Color systemAccent)
        {
            // update the theme if it has been changed elsewhere than in the settings
            if (CurrentTheme != currentTheme)
            {
                CurrentTheme = currentTheme;
                AppThemeName = ApplicationThemeManager.GetAppTheme().ToString();
                IsThemeSwitchChecked = (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark);
            }
        }

        [RelayCommand]
        private void OnToggleTheme()
        {
            ApplicationThemeManager.Apply(ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light);
            AppThemeName = ApplicationThemeManager.GetAppTheme().ToString();

            SettingsManager.Instance.Settings.General.CurrentTheme = ApplicationThemeManager.GetAppTheme().ToString();
        }

        private void NotifySettingsChangedSubscribers(string settingName)
        {
            // notify all subscribers
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(settingName));
        }
    }
}
