using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Models;
using Anti_Recoil_Application.Services;
using Anti_Recoil_Application.UserControls;
using Anti_Recoil_Application.UserControls.Dialogs;
using Anti_Recoil_Application.ViewModels.DialogViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DialogService _dialogService;
        private readonly HostProviderService _hostProviderService;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly RegisterViewModel _registerViewModel;
        private readonly HomeViewModel _homeViewModel;


        private string _username = string.Empty;
        private string _password = string.Empty;
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

            _registerViewModel = new RegisterViewModel(_dialogService, _hostProviderService, _mainWindowViewModel);
            _homeViewModel = new HomeViewModel(_dialogService, _hostProviderService);

        }

        private async void ExecuteLogin(object parameter)
        {
            try
            {
                _homeViewModel.LoadWeaponsAsync();
                _mainWindowViewModel.SwitchCurrentView(_homeViewModel);

                return;

                var (isAuthenticated, isConnectionIssue) = await _hostProviderService.AuthenticateAsync(Username, Password);

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
            var dialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                "Enter Your Username or Email",
                Username,
                "Enter Username or Email",
                async (enteredText) =>
                {


                    // Check if enteredText is a valid Username or Email
                    var user = await _hostProviderService.ValidateUsernameOrEmailAsync(enteredText);

                    if (user != null)
                    {
                        // If the enteredText is valid, send code to email
                        var isCodeSent = await _hostProviderService.SendVerificationCodeAsync(user.Email);

                        if (isCodeSent)
                        {
                            // Show confirmation dialog if the code was sent successfully
                            _mainWindowViewModel.IsLoading = true;
                            ShowConfirmationDialog(user);
                            _mainWindowViewModel.IsLoading = false;

                        }
                        else
                        {
                            // Handle the case where sending the code fails (e.g., show an error message)
                        }
                    }
                    else
                    {
                        // Handle the case where enteredText is invalid (e.g., show an error message)
                    }
                }
            );

            _dialogService.ShowDialog(dialogViewModel);
        }

        private void ShowConfirmationDialog(User user)
        {
            var confirmationDialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                "Please, Enter Code Sent to Your Mail.", 
                string.Empty,
                "Enter your code.",
                async (enteredText) =>
                {
                    // Check if the entered code is valid
                    var isCodeValid = await _hostProviderService.ValidateVerificationCodeAsync(user.Email, enteredText);

                    if (isCodeValid)
                    {
                        _mainWindowViewModel.IsLoading = true;
                        ShowForgotPasswordDialog(user, enteredText);
                        _mainWindowViewModel.IsLoading = false;

                    }
                    else
                    {

                    }
                }
            );

            _dialogService.ShowDialog(confirmationDialogViewModel);
        }

        private void ShowForgotPasswordDialog(User user, string enteredCode)
        {
            var forgotPasswordDialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                "Enter New Password.", 
                string.Empty,
                string.Empty, // Set to an empty string instead of null
                async (enteredText) =>
                {
                    var isCodeValid = await _hostProviderService.UpdatePasswordAsync(user.Email, enteredText, enteredCode);

                    // Update the password with the new one
                    _dialogService.CloseDialog();
                }
            );

            _dialogService.ShowDialog(forgotPasswordDialogViewModel, true);
        }

        private void ExecuteRegister(object parameter)
        {
            _mainWindowViewModel.SwitchCurrentView(_registerViewModel);
        }
    }
}
