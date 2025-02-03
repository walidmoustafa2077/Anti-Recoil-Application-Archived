using Anti_Recoil_Application.UserControls;
using Anti_Recoil_Application.ViewModels.DialogViewModels;

namespace Anti_Recoil_Application.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {


        private object? currentViewModel;
        private object? _currentDialogViewModel;

        private bool _isDialogVisible;
        private bool _isLoading;

        public object? CurrentView
        {
            get => currentViewModel;
            set => SetProperty(ref currentViewModel, value);
        }

        public object? CurrentDialog
        {
            get => _currentDialogViewModel;
            set => SetProperty(ref _currentDialogViewModel, value);
        }


        public bool IsDialogVisible
        {
            get => _isDialogVisible;
            set => SetProperty(ref _isDialogVisible, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Add logic to update dialog visibility/content if needed

        public void SwitchCurrentView(object? currentViewModel)
        {
            switch (currentViewModel)
            {
                case LoginViewModel loginViewModel:
                    // Display the LoginUserControl with its DataContext set to LoginViewModel.
                    CurrentView = new LoginUserControl();
                    (CurrentView as LoginUserControl)!.DataContext = loginViewModel;
                    break;

                case RegisterViewModel registerViewModel:
                    // Display the RegisterUserControl with its DataContext set to RegisterViewModel.
                    CurrentView = new RegisterUserControl();
                    (CurrentView as RegisterUserControl)!.DataContext = registerViewModel;
                    break;
                case HomeViewModel homeViewModel:
                    // Display the HomeUserControl with its DataContext set to HomeViewModel.
                    CurrentView = new HomeUserControl();
                    (CurrentView as HomeUserControl)!.DataContext = homeViewModel;
                    break;
                default:
                    // Handle any unsupported view model type.
                    throw new InvalidOperationException($"Unsupported view model: {currentViewModel.GetType().Name}");
            }
        }



        public async Task ShowDialogAsync(MainDialogViewModel dialogViewModel)
        {
            var tcs = new TaskCompletionSource<bool>(); // Create a TaskCompletionSource

            // Modify the CloseDialog action to complete the task when the dialog is closed
            CurrentDialog = new UserControls.Dialogs.MainDialog { DataContext = dialogViewModel };
            IsDialogVisible = true;

            // Wait for the dialog to close before continuing
            await tcs.Task;
        }

        public void ShowDialog(object dialogViewModel, bool isRetyped)
        {
            bool isEnterCodeDialog = false;

            // check if new dialog is header have Send Verification Code and if it is, then set isEnterCodeDialog to true
            if (dialogViewModel is MainDialogViewModel mainDialogViewModel && mainDialogViewModel.HeaderText.Contains("Verification Code"))
                isEnterCodeDialog = true;

            CurrentDialog = dialogViewModel switch
            {
                AccountDialogViewModel accountDialog => new UserControls.Dialogs.AccountDialog { DataContext = accountDialog },
                EnterFieldDialogViewModel enterFieldDialog => isRetyped
                    ? new UserControls.Dialogs.EnterRepeatedFieldDialog { DataContext = enterFieldDialog }
                    : isEnterCodeDialog ? new UserControls.Dialogs.EnterVerificationCodeDialog { DataContext = enterFieldDialog } : new UserControls.Dialogs.EnterFieldDialog(enterFieldDialog),
                ErrorDialogViewModel errorDialog => new UserControls.Dialogs.ErrorDialog { DataContext = errorDialog },
                MainDialogViewModel dialog => new UserControls.Dialogs.MainDialog { DataContext = dialog },
                _ => throw new ArgumentException("Unsupported dialog view model type.", nameof(dialogViewModel))
            };

            IsDialogVisible = true;
        }


        public async Task ShowDialogAsync(object dialogViewModel)
        {
            var tcs = new TaskCompletionSource<bool>(); // Create a TaskCompletionSource

            if (dialogViewModel == null)
                throw new ArgumentNullException(nameof(dialogViewModel));

            // Map the dialogViewModel to the appropriate dialog
            CurrentDialog = dialogViewModel switch
            {
                EnterFieldDialogViewModel enterFieldDialogViewModel =>
                    new UserControls.Dialogs.EnterFieldDialog(enterFieldDialogViewModel),
                ErrorDialogViewModel errorDialogViewModel =>
                    new UserControls.Dialogs.ErrorDialog { DataContext = errorDialogViewModel },
                MainDialogViewModel mainDialogViewModel =>
                    new UserControls.Dialogs.MainDialog { DataContext = mainDialogViewModel },

                _ => throw new InvalidOperationException($"Unsupported dialogViewModel type: {dialogViewModel.GetType().Name}")
            };

            IsDialogVisible = true;

            // Optionally, await a delay for demonstration purposes or consistency
            await tcs.Task;
        }


        public void CloseDialog()
        {
            // Ensure that the dialogViewModel is cleared
            CurrentDialog = null;
            IsDialogVisible = false;
        }

  
    }
}
