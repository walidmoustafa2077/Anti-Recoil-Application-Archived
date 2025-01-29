using Anti_Recoil_Application.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using Anti_Recoil_Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Anti_Recoil_Application.Helpers;
using Anti_Recoil_Application.Models;

namespace Anti_Recoil_Application.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly DialogService _dialogService;
        private readonly HostProviderService _hostProviderService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private string _firstName;
        private string _lastName;
        private string _username;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _country;
        private string _city;
        private DateTime? _dateOfBirth;
        private string _selectedGender;


        public RegisterViewModel(DialogService dialogService, HostProviderService hostProviderService, MainWindowViewModel mainWindowViewModel)
        {
            _dialogService = dialogService;
            _hostProviderService = hostProviderService;
            _mainWindowViewModel = mainWindowViewModel;

            RegisterCommand = new CommandBase(ExecuteRegister);
            CancelCommand = new CommandBase(ExecuteCancel);
        }


        // First Name Property
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value, nameof(FirstName)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Last Name Property
        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value, nameof(LastName)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Username Property
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value, nameof(Username)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Email Property
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value, nameof(Email)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Password Property
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value, nameof(Password)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Confirm Password Property
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value, nameof(ConfirmPassword)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Country Property
        public string Country
        {
            get => _country;
            set
            {
                if (SetProperty(ref _country, value, nameof(Country)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // City Property
        public string City
        {
            get => _city;
            set
            {
                if (SetProperty(ref _city, value, nameof(City)))
                {
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Date of Birth Property
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (SetProperty(ref _dateOfBirth, value, nameof(DateOfBirth)))
                {
                    OnPropertyChanged(nameof(DateOfBirth));
                    (RegisterCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }
        public string SelectedGender
        {
            get => _selectedGender;
            set
            {
                _selectedGender = value;
                OnPropertyChanged(nameof(SelectedGender)); // Notify property change if using INotifyPropertyChanged
                }
        }
        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }



        private async void ExecuteRegister(object parameter)
        {
            // Validate the user input
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword) ||
                string.IsNullOrEmpty(Country) || string.IsNullOrEmpty(City) || !DateOfBirth.HasValue || string.IsNullOrEmpty(SelectedGender))
            {
                // Show error message if any required field is empty
                _dialogService.ShowDialog("Please fill in all required fields.");
                return;
            }

            // Validate email format and domain
            if (!ValidatedEmail.IsValidEmail(Email))
            {
                _dialogService.ShowDialog("Please enter a valid email address (Gmail, Outlook, iCloud only).");
                return;
            }

            // Validate that password and confirm password match
            if (Password != ConfirmPassword)
            {
                // Show error message if passwords do not match
                _dialogService.ShowDialog("Passwords do not match.");
                return;
            }

            // Validate password strength
            if (!PasswordHelper.IsPasswordStrong(Password))
            {
                _dialogService.ShowDialog("Password is too weak. It must contain at least 8 characters, including a mix of uppercase, lowercase, digits, and special characters.");
                return;
            }

            // Minimum age validation (e.g., 18 years old)
            var minimumAge = 18;
            if (DateOfBirth.HasValue)
            {
                var age = DateTime.Now.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > DateTime.Now.AddYears(-age)) age--; // Adjust for if birthday hasn't occurred yet this year

                if (age < minimumAge)
                {
                    _dialogService.ShowDialog($"You must be at least {minimumAge} years old.");
                    return;
                }
            }

            // Ensure the DateOfBirth is not more than 150 years ago
            var maxAllowedDate = DateTime.Now.AddYears(-150); // 150 years ago
            if (DateOfBirth.HasValue && DateOfBirth.Value < maxAllowedDate)
            {
                _dialogService.ShowDialog("Date of Birth cannot be more than 150 years ago.");
                return;
            }

            // Call the RegisterNewUser method in HostProviderService
            var (registrationSuccess, email) = await _hostProviderService.RegisterNewUser(
                FirstName,
                LastName,
                Username,
                Email,
                Password,
                DateOfBirth,
                SelectedGender,
                Country,
                City,
                this
            );

            if (registrationSuccess)
            {
                // Handle successful registration (e.g., switch to login screen)
                _mainWindowViewModel.SwitchCurrentView(App.AppHost?.Services.GetRequiredService<LoginViewModel>());
            }
        }

        public void HasRegistered()
        {
            // Handle successful registration (e.g., switch to login screen)
            _mainWindowViewModel.SwitchCurrentView(App.AppHost?.Services.GetRequiredService<LoginViewModel>());
        }


        private void ExecuteCancel(object obj)
        {
            // Handle successful registration (e.g., switch to login screen)
            _mainWindowViewModel.SwitchCurrentView(App.AppHost?.Services.GetRequiredService<LoginViewModel>());
        }
    }

}
