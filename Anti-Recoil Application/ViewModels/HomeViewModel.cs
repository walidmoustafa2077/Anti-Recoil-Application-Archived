using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Core.Services;
using Anti_Recoil_Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;

namespace Anti_Recoil_Application.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly HostProviderService _hostProviderService;
        private readonly DialogService _dialogService;
        private readonly CoreService _coreService;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private string licenseType = $"Pro";

        private WeaponViewModel _selectedWeapon = new WeaponViewModel();

        private bool _isActivateChecked;

        private DateTime? _startTime;

        public HomeViewModel(DialogService dialogService, HostProviderService hostProviderService, MainWindowViewModel mainWindowViewModel)
        {
            _dialogService = dialogService;
            _hostProviderService = hostProviderService;

            _coreService = new CoreService(this);

            ActivateWeaponCommand = new CommandBase(ActivateWeapon);
            _mainWindowViewModel = mainWindowViewModel;
        }

        public string LicenseType
        {
            get => licenseType;
            set => SetProperty(ref licenseType, value, nameof(LicenseType));
        }

        public ObservableCollection<WeaponViewModel> Weapons { get; set; } = new ObservableCollection<WeaponViewModel>();

        public WeaponViewModel SelectedWeapon
        {
            get => _selectedWeapon;
            set
            {
                if (SetProperty(ref _selectedWeapon, value, nameof(SelectedWeapon)) && value != null)
                {
                    // Use the Dispatcher to safely update data-bound properties
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        // If already on the UI thread, update directly
                        // Update activation status for all weapons
                        foreach (var w in Weapons)
                        {
                            w.IsActive = w == value;
                        }
                    }
                    else
                    {
                        // Marshal the update to the UI thread
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // Update activation status for all weapons
                            foreach (var w in Weapons)
                            {
                                w.IsActive = w == value;
                            }
                        });
                    }
                }
            }
        }


        public bool IsActivateChecked
        {
            get => _isActivateChecked;
            set
            {
                // Use the Dispatcher to safely update data-bound properties
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    // If already on the UI thread, update directly
                    if (SetProperty(ref _isActivateChecked, value, nameof(IsActivateChecked)))
                    {
                        if (string.IsNullOrEmpty(SelectedWeapon.WeaponName))
                        {
                            // Show a dialog to the user to select a weapon
                            _dialogService.ShowDialog("Please select a weapon to activate.");
                            IsActivateChecked = false;
                        }
                        else
                        {
                            // Activate or deactivate the selected weapon
                            _coreService.IsActive = value;
                        }
                    }
                }
                else
                {
                    // Marshal the update to the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (SetProperty(ref _isActivateChecked, value, nameof(IsActivateChecked)))
                        {
                            if (string.IsNullOrEmpty(SelectedWeapon.WeaponName))
                            {
                                // Show a dialog to the user to select a weapon
                                _dialogService.ShowDialog("Please select a weapon to activate.");
                                IsActivateChecked = false;
                            }
                            else
                            {
                                // Activate or deactivate the selected weapon
                                _coreService.IsActive = value;
                            }
                        }
                    });
                }
            }
        }

        public DateTime? StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value, nameof(StartTime));
        }


        public CommandBase ActivateWeaponCommand { get; }


        public async void LoadWeaponsAsync()
        {
            try
            {
                var weapons = await _hostProviderService.GetWeaponsAsync();
                Weapons.Clear();
                for (int i = 0; i < weapons.Count; i++)
                {
                    var weapon = weapons[i];
                    Weapons.Add(new WeaponViewModel
                    {
                        WeaponName = weapon.WeaponName,
                        WeaponImage = $"pack://application:,,,/Resources/Images/Weapons/{weapon.WeaponName}.png",
                        Shortcut = $"F{i + 1}", // Assign shortcut keys based on the order (F1, F2, etc.)
                        IsActive = false
                    });
                }

                // Initialize weapons in the CoreService
                _coreService.InitializeWeapons(weapons);
                _coreService.UpdateLoop();
            }
            catch (HttpRequestException ex)
            {
                _dialogService.ShowErrorDialog(ex.Message, Logout, "Retry!", LoadWeaponsAsync);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog(ex.Message, Logout, "Retry!", LoadWeaponsAsync);
            }

        }

        private void Logout()
        {
            Microsoft.Extensions.Hosting.IHost? appHost = App.AppHost;
            
            if (appHost == null)
            {
                throw new ArgumentNullException(nameof(appHost));
            }

            var dialogService = appHost.Services.GetRequiredService<DialogService>();
            var hostProviderService = appHost.Services.GetRequiredService<HostProviderService>();
            var mainWindowViewModel = appHost.Services.GetRequiredService<MainWindowViewModel>();
            _mainWindowViewModel.SwitchCurrentView(new LoginViewModel(dialogService, hostProviderService, mainWindowViewModel));    
        }

        private void ActivateWeapon(object parameter)
        {
            if (parameter is WeaponViewModel weapon)
            {
                if (SelectedWeapon != weapon)
                {
                    SelectedWeapon = weapon;

                    // Update activation status for all weapons
                    foreach (var w in Weapons)
                    {
                        w.IsActive = w == weapon;
                    }
                }
                else if (SelectedWeapon.IsActive)
                {
                    // Deactivate the selected weapon
                    SelectedWeapon.IsActive = false;
                    SelectedWeapon = new WeaponViewModel();
                }
            }
        }

    }
}
