using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Core.Services;
using Anti_Recoil_Application.Enums;
using Anti_Recoil_Application.Models;
using Anti_Recoil_Application.Services;
using Anti_Recoil_Application.UserControls.Dialogs;
using Anti_Recoil_Application.ViewModels.DialogViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly HostProviderService _hostProviderService;
        private readonly DialogService _dialogService;
        private readonly CoreService _coreService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private string _licenseType = $"Pro";
        public User _currentUser { get; private set; } = new User();

        private WeaponViewModel _selectedWeapon = new WeaponViewModel();

        private bool _isActivateChecked;

        private DateTime? _startTime;


        public HomeViewModel(DialogService dialogService, HostProviderService hostProviderService, MainWindowViewModel mainWindowViewModel)
        {
            _dialogService = dialogService;
            _hostProviderService = hostProviderService;
            _mainWindowViewModel = mainWindowViewModel;
            _coreService = new CoreService(this);

            ActivateWeaponCommand = new CommandBase(ActivateWeapon);

            LogoutCommand = new CommandBase(Logout);
            AccountCommand = new CommandBase(AccountDialog);
            SettingsCommand = new CommandBase(SettingsDialog);

            // Start async initialization without blocking
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                User? user = await _hostProviderService.GetUserAsync();
                if (user == null)
                {
                    throw new Exception("Failed to load user.");
                }
                _currentUser = user;
                LicenseType = user.LicenseType;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Failed to load user: {ex.Message}");
            }
        }

        public string LicenseType
        {
            get => _licenseType;
            set => SetProperty(ref _licenseType, value, nameof(LicenseType));
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


        // Logout Command
        public ICommand LogoutCommand { get; }

        // Account Command
        public ICommand AccountCommand { get; }

        // Settings Command
        public ICommand SettingsCommand { get; }


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
                _dialogService.ShowErrorDialog(ex.Message, () => Logout(null), "Retry!", LoadWeaponsAsync);
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog(ex.Message, () => Logout(null), "Retry!", LoadWeaponsAsync);
            }

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

        private void Logout(object? obj)
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


        private void AccountDialog(object? obj)
        {
            // Show the Account dialog

            // create a new instance of the Account dialog
            var accountDialog = new AccountDialogViewModel(
                onCloseButtonClick: () => _dialogService.CloseDialog(),
                onSubmitButtonClick: () =>
                {
                    // Save the changes
                    // check if the user has made any changes
                    var accountDialog = _mainWindowViewModel.CurrentDialog as AccountDialog;
                    if (accountDialog == null)
                        return;
                    var accountDialogViewModel = accountDialog.DataContext as AccountDialogViewModel;

                    if (accountDialogViewModel == null)
                        return;
                    
                    _dialogService.CloseDialog();


                    // if user has updated first name
                    if (!string.IsNullOrEmpty(accountDialogViewModel.FirstName) && accountDialogViewModel.FirstName != _currentUser.FirstName)
                    {
                        _currentUser.FirstName = accountDialogViewModel.FirstName;
                        UpdateUser(UpdateUserOption.FirstName);
                    }
                    // if user has updated last name
                    if (!string.IsNullOrEmpty(accountDialogViewModel.LastName) && accountDialogViewModel.LastName != _currentUser.LastName)
                    {
                        _currentUser.LastName = accountDialogViewModel.LastName;
                        UpdateUser(UpdateUserOption.LastName);
                    }
                    // if user has updated username
                    if (!string.IsNullOrEmpty(accountDialogViewModel.Username) && accountDialogViewModel.Username != _currentUser.Username)
                    {
                        _currentUser.Username = accountDialogViewModel.Username;
                        UpdateUser(UpdateUserOption.Username);
                    }
                    // if user has updated email
                    if (!string.IsNullOrEmpty(accountDialogViewModel.Email) && accountDialogViewModel.Email != _currentUser.Email)
                    {
                        _currentUser.Email = accountDialogViewModel.Email;
                        UpdateUser(UpdateUserOption.Email);
                    }
                },
                onLicenseButtonClick: () =>
                {
                    // Show the License dialog
                    _dialogService.CloseDialog();
                },
                onUpdatePasswordButtonClick: () =>
                {
                    _dialogService.CloseDialog();
                    // Show the Update Password dialog
                    CheckPasswordDialog(UpdatePasswordDialog);
                })
            {
                HeaderText = "Account",
                FirstNameWatermarkText = _currentUser.FirstName,
                LastNameWatermarkText = _currentUser.LastName,
                UsernameWatermarkText = _currentUser.Username,
                EmailWatermarkText = _currentUser.Email,
                LicenseButtonText = _currentUser.LicenseType,
                SubmitButtonText = "Save",
                ButtonText = "Close"
            };

            _dialogService.ShowDialog(accountDialog);
        }

        private void UpdateUser(UpdateUserOption option)
        {
            CheckPasswordDialog(async () => {
                switch (option)
                {
                    case UpdateUserOption.FirstName:
                        await _hostProviderService.UpdateUserAsync(_currentUser.Email, _currentUser.FirstName, option);
                        break;
                    case UpdateUserOption.LastName:
                        await _hostProviderService.UpdateUserAsync(_currentUser.Email, _currentUser.LastName, option);
                        break;
                    case UpdateUserOption.Username:
                        await _hostProviderService.UpdateUserAsync(_currentUser.Email, _currentUser.Username, option);
                        break;
                    case UpdateUserOption.Email:
                        await _hostProviderService.UpdateUserAsync(_currentUser.Email, _currentUser.Email, option);
                        break;
                    default:
                        break;
                }
            });
        }



        private void CheckPasswordDialog(Action? updateAction = null)
        {
            var checkPasswordDialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                "Enter Password.",
                string.Empty,
                string.Empty, // Set to an empty string instead of null
                async (enteredText) =>
                {
                    var isCodeValid = await _hostProviderService.CheckPasswordAsync(_currentUser.Email, enteredText);

                    if (isCodeValid && updateAction != null)
                    {
                        _dialogService.CloseDialog();
                        updateAction();
                    }
                    else
                    {
                        _dialogService.CloseDialog();
                        _dialogService.ShowErrorDialog("Invalid Password.");
                    }
                }
            );

            _dialogService.ShowDialog(checkPasswordDialogViewModel);
        }


        private void UpdatePasswordDialog()
        {
            var forgotPasswordDialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                "Enter New Password.",
                string.Empty,
                string.Empty, // Set to an empty string instead of null
                (enteredText) =>
                {
     
                },
                async (enteredText, secondEnteredText) =>
                {
                    if (enteredText != secondEnteredText)
                    {
                        _dialogService.ShowErrorDialog("Passwords do not match.");
                        return;
                    }

                    var isCodeValid = await _hostProviderService.UpdateUserAsync(_currentUser.Email, enteredText, Enums.UpdateUserOption.Password);

                    // Update the password with the new one
                    _dialogService.CloseDialog();
                }
            );

            _dialogService.ShowDialog(forgotPasswordDialogViewModel, true);
        }

        // 
        private void SettingsDialog(object? obj)
        {

        }
    }
}
