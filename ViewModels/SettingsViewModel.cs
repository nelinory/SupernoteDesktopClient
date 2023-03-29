using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private ThemeType _currentTheme = ThemeType.Unknown;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        public SettingsViewModel()
        {
            CurrentTheme = Theme.GetAppTheme();

            Theme.Changed += OnThemeChanged;
        }

        private void OnThemeChanged(ThemeType currentTheme, Color systemAccent)
        {
            // Update the theme if it has been changed elsewhere than in the settings.
            if (CurrentTheme != currentTheme)
                CurrentTheme = currentTheme;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ThemeType.Light)
                        break;

                    Theme.Apply(ThemeType.Light);
                    CurrentTheme = ThemeType.Light;

                    break;

                default:
                    if (CurrentTheme == ThemeType.Dark)
                        break;

                    Theme.Apply(ThemeType.Dark);
                    CurrentTheme = ThemeType.Dark;

                    break;
            }
        }
    }
}
