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

                            services.AddSingleton<DialogService>();
                            services.AddSingleton<HostProviderService>();

                            services.AddSingleton<LoginViewModel>();
                            services.AddSingleton<RegisterViewModel>();


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

            // Set the initial view to LoginUserControl via MainWindowViewModel
            var mainWindowViewModel = AppHost.Services.GetRequiredService<MainWindowViewModel>();
            mainWindowViewModel.SwitchCurrentView(AppHost.Services.GetRequiredService<LoginViewModel>());

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
