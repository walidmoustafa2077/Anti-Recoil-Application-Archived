using Anti_Recoil_Application.Core.Services;
using Anti_Recoil_Application.Services;
using Anti_Recoil_Application.UserControls;
using Anti_Recoil_Application.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Anti_Recoil_Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            // Build the host with DI setup
            AppHost = Host.CreateDefaultBuilder()
                        .ConfigureServices((hostContext, services) =>
                        {
                            // Register your ViewModels, Views, and Services
                            services.AddSingleton<MainWindowViewModel>();

                            services.AddSingleton<SettingsService>();
                            services.AddSingleton<DialogService>();
                            services.AddSingleton<HostProviderService>();

                            // Add other services, etc.
                            services.AddTransient<MainWindow>();
                        })
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole();
                        })
                        .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            var settingsService = AppHost.Services.GetRequiredService<SettingsService>();
            var dialogService = AppHost.Services.GetRequiredService<DialogService>();
            var hostProviderService = AppHost.Services.GetRequiredService<HostProviderService>();

            // Set the initial view to LoginUserControl via MainWindowViewModel
            var mainWindowViewModel = AppHost.Services.GetRequiredService<MainWindowViewModel>();

            settingsService.Load();

            // get token from settings
            var token = settingsService.Settings.Token;

            if (string.IsNullOrEmpty(token))
            {
                // If the token is empty, display the LoginUserControl
                var loginViewModel = new LoginViewModel(dialogService, hostProviderService, mainWindowViewModel);
                mainWindowViewModel.SwitchCurrentView(loginViewModel);
            }
            else
            {
                // If the token is not empty, display the HomeUserControl
                var homeViewModel = new HomeViewModel(dialogService, hostProviderService, mainWindowViewModel);
                await homeViewModel.InitializeAsync();
                homeViewModel.LoadWeaponsAsync();
                mainWindowViewModel.SwitchCurrentView(homeViewModel);
            }

            mainWindow.Show();

            base.OnStartup(e);

        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            AppHost.Dispose(); // Ensure cleanup of resources

            base.OnExit(e);
        }
    }
}
