using Anti_Recoil_Application.Commands;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    class AccountDialogViewModel : EnterFieldDialogViewModel
    {
        private string _firstName = string.Empty;
        private string _firstNameWatermarkTextName = string.Empty;
        private string _lastName = string.Empty;
        private string _lastNameWatermarkTextName = string.Empty;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _licenseButtonText = string.Empty;
        private string _emailWatermarkTextName;
        private string _usernameWatermarkTextName;

        public AccountDialogViewModel(Action onCloseButtonClick, Action? onSubmitButtonClick = null, Action? onResendButtonClick = null, 
            Action? onLicenseButtonClick = null, Action? onUpdatePasswordButtonClick = null) : 
            base(onCloseButtonClick, onSubmitButtonClick, onResendButtonClick)
        {
            LicenseCommand = new CommandBase(_ => onLicenseButtonClick?.Invoke());
            UpdatePasswordCommand = new CommandBase(_ => onUpdatePasswordButtonClick?.Invoke());
        }

        // First Name Property
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value, nameof(FirstName)))
                {
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // First Name Property
        public string FirstNameWatermarkText
        {
            get => _firstNameWatermarkTextName;
            set
            {
                if (SetProperty(ref _firstNameWatermarkTextName, value, nameof(FirstNameWatermarkText)))
                {
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
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
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }
        // Last Name Property
        public string LastNameWatermarkText
        {
            get => _lastNameWatermarkTextName;
            set
            {
                if (SetProperty(ref _lastNameWatermarkTextName, value, nameof(LastNameWatermarkText)))
                {
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
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
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string UsernameWatermarkText
        {
            get => _usernameWatermarkTextName;
            set
            {
                if (SetProperty(ref _usernameWatermarkTextName, value, nameof(UsernameWatermarkText)))
                {
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
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
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }
        public string EmailWatermarkText
        {
            get => _emailWatermarkTextName;
            set
            {
                if (SetProperty(ref _emailWatermarkTextName, value, nameof(EmailWatermarkText)))
                {
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string LicenseButtonText
        {
            get => _licenseButtonText;
            set
            {
                if (SetProperty(ref _licenseButtonText, value, nameof(LicenseButtonText)) && value != "Pro")
                {
                    (SubmitCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        // Submit command
        public ICommand LicenseCommand { get; }
        // Resend command
        public ICommand UpdatePasswordCommand { get; }

    }
}
