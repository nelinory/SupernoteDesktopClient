using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Core.Win32Api;
using SupernoteDesktopClient.Views.Pages;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        // main window handle
        private IntPtr _windowHandle;

        public ViewModels.MainWindowViewModel ViewModel { get; }

        public MainWindow(ViewModels.MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService, ISnackbarService snackbarService, IContentDialogService contentDialogService)
        {
            ViewModel = viewModel;
            DataContext = this;

            Loaded += OnLoaded;
            Closing += OnClosing;

            InitializeComponent();
            SetPageService(pageService);

            navigationService.SetNavigationControl(RootNavigation);
            snackbarService.SetSnackbarPresenter(RootSnackbar);
            contentDialogService.SetDialogHost(RootDialog);

            ApplicationThemeManager.Apply((ApplicationTheme)Enum.Parse(typeof(ApplicationTheme), SettingsManager.Instance.Settings.General.CurrentTheme), WindowBackdropType.Mica, true);
        }

        #region INavigationWindow methods

        //public Frame GetFrame() => RootFrame;

        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // make sure that closing this window will begin the process of closing the application
            Application.Current.Shutdown();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // restore the window position
            if (SettingsManager.Instance.Settings.General.RememberAppWindowPlacement == true)
            {
                WindowPlacement windowPlacement = SettingsManager.Instance.Settings.General.AppWindowPlacement;
                NativeMethods.SetWindowPlacementEx(_windowHandle, ref windowPlacement);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // save the main window handle
            _windowHandle = new WindowInteropHelper(this).Handle;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            // remember the window position
            if (SettingsManager.Instance.Settings.General.RememberAppWindowPlacement == true)
                SettingsManager.Instance.Settings.General.AppWindowPlacement = NativeMethods.GetWindowPlacementEx(_windowHandle);

            // save settings before exiting
            SettingsManager.Instance.Save();

            // cleanup temp conversion files
            FileSystemManager.CleanupTempConversionFiles();
        }

        private void RootNavigation_Navigated(INavigationView sender, NavigatedEventArgs e)
        {
            if (sender is not NavigationView)
                return;

            // hide breadcrumb header for target DashboardPage
            BreadcrumbBar.Visibility = (e.Page.GetType() == typeof(DashboardPage)) ? Visibility.Collapsed : Visibility.Visible;
        }

        #region NotifyIcon Context Menu

        // this is not following MVVM, due to the inability of the RelayCommand to get data bind context for NotifyIcon context menu

        private void NotifyIcon_LeftDoubleClick(Wpf.Ui.Tray.Controls.NotifyIcon sender, RoutedEventArgs e)
        {
            ShowApplicationWindow();
        }

        private void NotifyIcon_MenuItemClick(object sender, RoutedEventArgs e)
        {
            switch (((FrameworkElement)sender).Tag.ToString())
            {
                case "home":
                    Navigate(typeof(DashboardPage));
                    ShowApplicationWindow();
                    break;
                case "sync":
                    Navigate(typeof(SyncPage));
                    ShowApplicationWindow();
                    break;
                case "settings":
                    Navigate(typeof(SettingsPage));
                    ShowApplicationWindow();
                    break;
                default: // exit
                    this.CloseWindow();
                    break;
            }
        }

        private void ShowApplicationWindow()
        {
            // show the minimized to tray main window
            if (this.Visibility == Visibility.Hidden)
                this.ShowWindow();
            else
                this.Activate();

            if (this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}