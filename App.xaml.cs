using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SupernoteDesktopClient.Core;
using SupernoteDesktopClient.Core.Win32Api;
using SupernoteDesktopClient.Services;
using SupernoteDesktopClient.Services.Contracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace SupernoteDesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly Mutex _appMutex = new Mutex(true, "C5FDA39A-40DA-4C77-842B-0C878F0D73C2");
        private static readonly string _logsPath = Path.Combine(FileSystemManager.GetApplicationFolder(), "Logs");

        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // App Host
                services.AddHostedService<ApplicationHostService>();

                // Framework services
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<ITaskBarService, TaskBarService>();
                services.AddSingleton<ISnackbarService, SnackbarService>();

                // Custom services
                services.AddSingleton<IUsbHubDetector, UsbHubDetector>();
                services.AddSingleton<IMediaDeviceService, MediaDeviceService>();
                services.AddSingleton<ISyncService, SyncService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddScoped<INavigationWindow, Views.Windows.MainWindow>();
                services.AddScoped<ViewModels.MainWindowViewModel>();

                // Views and ViewModels
                services.AddScoped<Views.Pages.AboutPage>();
                services.AddScoped<ViewModels.AboutViewModel>();
                services.AddScoped<Views.Pages.DashboardPage>();
                services.AddScoped<ViewModels.DashboardViewModel>();
                services.AddScoped<Views.Pages.SettingsPage>();
                services.AddScoped<ViewModels.SettingsViewModel>();
                services.AddScoped<Views.Pages.SyncPage>();
                services.AddScoped<ViewModels.SyncViewModel>();
            })
            .UseSerilog()
            .Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            ForceSingleInstance();

            ConfigureLogging();

            SetupUnhandledExceptionHandling();

            await _host.StartAsync();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            // flush all log items before exit
            Log.CloseAndFlush();

            await _host.StopAsync();

            _host.Dispose();
        }

        private static void ForceSingleInstance()
        {
            if (_appMutex.WaitOne(TimeSpan.Zero, true) == true)
            {
                _appMutex.ReleaseMutex();

                // show splash window
                SplashScreen splash = new SplashScreen(@"assets\spash.png");
                splash.Show(true, true);
            }
            else
            {
                Process[] processes = Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name);
                {
                    if (processes.Length > 0)
                    {
                        NativeMethods.ShowWindowEx(processes[0].MainWindowHandle, NativeMethods.SW_RESTORE_WINDOW);
                        NativeMethods.SetForegroundWindowEx(processes[0].MainWindowHandle);
                    }
                }

                Application.Current.Shutdown();
            }
        }

        private static void ConfigureLogging()
        {
            // configure logging
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(Path.Combine(_logsPath, "Sdc-.log"),
                                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                                outputTemplate: "{Timestamp:MM/dd/yyyy HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                                rollingInterval: RollingInterval.Day,
                                retainedFileCountLimit: 7)
               .CreateLogger();
        }

        private void SetupUnhandledExceptionHandling()
        {
            // handler for all exceptions from all threads - can recover
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e)
            {
                ShowExceptionAndExit(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");
            };

            // handler for exceptions from each AppDomain that uses a task scheduler for async operations - can recover
            TaskScheduler.UnobservedTaskException += delegate (object sender, UnobservedTaskExceptionEventArgs e)
            {
                ShowExceptionAndExit(e.Exception, "TaskScheduler.UnobservedTaskException");
            };

            // handler for all exceptions from a single dispatcher thread - cannot recover
            Current.DispatcherUnhandledException += delegate (object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it
                if (Debugger.IsAttached == false)
                {
                    e.Handled = true;
                    ShowExceptionAndExit(e.Exception, "Current.DispatcherUnhandledException");
                }
            };
        }

        private void ShowExceptionAndExit(Exception ex, string exceptionType)
        {
            Log.Error("Fatal application exception: {EX}", ex);

            string errorMessage = $"A fatal application error occurred: {ex.Message}\n\nPlease, check error logs at:\n{_logsPath} for more details.\n\nApplication will close now.";
            MessageBox.Show(errorMessage, $"Sdc - Fatal Error: {exceptionType}", MessageBoxButton.OK, MessageBoxImage.Error);

            try
            {
                SettingsManager.Instance.Save();
            }
            catch (Exception)
            {
                // ignore we are exiting anyway
            }

            Application.Current.Shutdown();
        }
    }
}