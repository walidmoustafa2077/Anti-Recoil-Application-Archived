using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Services;
using Anti_Recoil_Application.ViewModels.DialogViewModels;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DialogService _dialogService;
        private readonly HostProviderService _hostProviderService;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private string _username;
        private string _password;
        private bool _isLoggingIn;

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value, nameof(Username)))
                {
                    (LoginCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value, nameof(Password)))
                {
                    (LoginCommand as CommandBase)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(ref _isLoggingIn, value, nameof(IsLoggingIn));
        }

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(DialogService dialogService, HostProviderService loginService, MainWindowViewModel mainWindowViewModel)
        {
            _dialogService = dialogService;
            _hostProviderService = loginService;
            _mainWindowViewModel = mainWindowViewModel;

            LoginCommand = new CommandBase(ExecuteLogin);
            ForgotPasswordCommand = new CommandBase(ExecuteForgotPassword);
            RegisterCommand = new CommandBase(ExecuteRegister);
        }

        private async void ExecuteLogin(object parameter)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;
                var (isAuthenticated, isConnectionIssue) = await _hostProviderService.AuthenticateAsync(Username, Password);
                _mainWindowViewModel.IsLoading = false;

                if (isConnectionIssue)
                    return;

                var dialogViewModel = new MainDialogViewModel(() =>
                {
                    _dialogService.CloseDialog();
                })
                {
                    HeaderText = isAuthenticated ? "Login successful!" : "Invalid username or password.",
                    ButtonText = isAuthenticated ? "Close" : "Retry"
                };

                IsLoggingIn = true;

                _dialogService.ShowDialog(dialogViewModel);
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        private void ExecuteForgotPassword(object obj)
        {
            var dialogViewModel = CreateEnterFieldDialogViewModel(
                "Enter Your Username or Email",
                Username,
                () => ShowConfirmationDialog()
            );

            _dialogService.ShowDialog(dialogViewModel);
        }

        private void ShowConfirmationDialog()
        {
            var confirmationDialogViewModel = CreateEnterFieldDialogViewModel(
                "Please, Enter Code Sent to Your Mail.",
                null,
                () => ShowForgotPasswordDialog()
            );

            _dialogService.ShowDialog(confirmationDialogViewModel);
        }

        private void ShowForgotPasswordDialog()
        {
            var forgotPasswordDialogViewModel = CreateEnterFieldDialogViewModel(
                "Enter New Password.",
                null,
                () =>
                {
                    _hostProviderService.UpdatePassword(Username, Password);
                    _dialogService.CloseDialog();
                }
            );

            _dialogService.ShowDialog(forgotPasswordDialogViewModel, true);
        }

        private EnterFieldDialogViewModel CreateEnterFieldDialogViewModel(string headerText, string mainField, Action onSubmit)
        {
            return new EnterFieldDialogViewModel(
                onButtonClick: () => _dialogService.CloseDialog(),
                onSubmitButtonClick: () =>
                {
                    _dialogService.CloseDialog();
                    onSubmit();
                })
            {
                HeaderText = headerText,
                MainField = mainField,
                SubmitButtonText = "Submit",
                ButtonText = "Close"
            };
        }

        private void ExecuteRegister(object parameter)
        {
            var dialogViewModel = new MainDialogViewModel(() =>
            {
                _dialogService.CloseDialog();
            })
            {
                HeaderText = "Register functionality is not yet implemented.",
                ButtonText = "Close"
            };

            _dialogService.ShowDialog(dialogViewModel);
        }
    }
}
