using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Controls.Navigation;
using Wpf.Ui.Mvvm.Contracts;

namespace SupernoteDesktopClient.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        public MainWindowViewModel()
        {
            ApplicationTitle = "Supernote Desktop Client";
            
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Dashboard",
                    PageTag = "dashboard",
                    ToolTip = "Dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationSeparator(),
                //new NavigationItem()
                //{
                //    Content = "Note",
                //    PageTag = "note",
                //    Icon = SymbolRegular.Notebook24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "Document",
                //    PageTag = "document",
                //    Icon = SymbolRegular.DocumentText24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "Screenshot",
                //    PageTag = "screenshot",
                //    Icon = SymbolRegular.Image24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "Export",
                //    PageTag = "export",
                //    Icon = SymbolRegular.FolderArrowLeft24,
                //    IsEnabled = false
                //    //PageType = typeof(Views.Pages.DataPage)
                //},
                //new NavigationItem()
                //{
                //    Content = "MyStyle",
                //    PageTag = "mystyle",
                //    IsEnabled = false,
                //    Icon = SymbolRegular.TextBulletListSquareEdit24,
                //    //PageType = typeof(Views.Pages.DataPage)
                //}
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Theme",
                    ToolTip = "Theme",
                    Icon = SymbolRegular.DarkTheme24,
                    Command = new RelayCommand(ToggleTheme)
                },
                new NavigationItem()
                {
                    Content = "Settings",
                    PageTag = "settings",
                    ToolTip = "Settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(Views.Pages.SettingsPage)
                },
                new NavigationSeparator(),
                new NavigationItem()
                {
                    Content = "About",
                    PageTag = "about",
                    ToolTip = "About",
                    Icon = SymbolRegular.QuestionCircle24,
                    PageType = typeof(Views.Pages.AboutPage)
                }
            };
        }

        private void ToggleTheme()
        {
            Theme.Apply(Theme.GetAppTheme() == ThemeType.Light ? ThemeType.Dark : ThemeType.Light);
        }
    }
}
