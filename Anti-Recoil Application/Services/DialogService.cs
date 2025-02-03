using Anti_Recoil_Application.UserControls.Dialogs;
using Anti_Recoil_Application.ViewModels;
using Anti_Recoil_Application.ViewModels.DialogViewModels;

namespace Anti_Recoil_Application.Services
{
    public class DialogService
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public DialogService(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public void ShowDialog(object dialogViewModel)
        {
            _mainWindowViewModel.ShowDialog(dialogViewModel, false);
        }

        public void ShowDialog(object dialogViewModel, bool isRetyped)
        {
            _mainWindowViewModel.ShowDialog(dialogViewModel, true);
        }

        public async Task ShowDialogAsync(object dialogViewModel)
        {
            await _mainWindowViewModel.ShowDialogAsync(dialogViewModel);
        }

        public void ShowDialog(string message)
        {
            var dialogViewModel = new MainDialogViewModel(() => CloseDialog())
            {
                HeaderText = message,
                ButtonText = "Close"
            };

            ShowDialog(dialogViewModel);
        }

        public void ShowErrorDialog(string message, Action? mainAction = null, string? secondButton = null, Action? secondAction = null)
        {
            bool haveSecondButton = !string.IsNullOrEmpty(secondButton) || secondAction != null;
            var dialogViewModel = new ErrorDialogViewModel(
                onButtonClick: () =>
                {
                    CloseDialog();
                    mainAction?.Invoke();
                },
                onSecondButtonClick: () =>
                {
                    // Close the dialog and invoke the second button action if provided
                    CloseDialog();
                    secondAction?.Invoke();
                })
            {
                HeaderText = message,
                ButtonText = "Close",
                SecondButtonText = haveSecondButton ? secondButton ?? "Retry" : string.Empty // Default to "Retry" if no text is provided
            };

            ShowDialog(dialogViewModel);
        }


        public async Task ShowDialogAsync(string message)
        {
            var dialogViewModel = new MainDialogViewModel(() => CloseDialog())
            {
                HeaderText = message,
                ButtonText = "Close"
            };

            await _mainWindowViewModel.ShowDialogAsync(dialogViewModel);
        }


        public async Task ShowErrorDialogAsync(string message)
        {
            var dialogViewModel = new ErrorDialogViewModel(() => CloseDialog())
            {
                HeaderText = message,
                ButtonText = "Close"
            };

            await _mainWindowViewModel.ShowDialogAsync(dialogViewModel);
        }

        public EnterFieldDialogViewModel CreateEnterFieldDialogViewModel(
            string headerText,
            string mainField,
            string fieldHelperText,
            Action<string?>? onSubmit = null,  // Single-argument version
            Action<string?, string?>? onSubmitDouble = null,  // Two-argument version
            Action<string>? onSecondSubmit = null)
        {
            return new EnterFieldDialogViewModel(
                onCloseButtonClick: () => CloseDialog(),
                onSubmitButtonClick: () =>
                {
                    string? mainFieldValue = null;  // Declare the variable outside of the if block
                    string? secondFieldValue = null;  // Declare the variable outside of the if block
                    var currentDialog = _mainWindowViewModel.CurrentDialog;

                    // Check if currentDialog is of type Send EnterFieldDialog or EnterRepeatedFieldDialog
                    if (currentDialog is EnterVerificationCodeDialog enterVerificationCodeDialog)
                    {
                        mainFieldValue = enterVerificationCodeDialog.DataContext?.GetType()
                            .GetProperty("MainField")?.GetValue(enterVerificationCodeDialog.DataContext) as string;
                        secondFieldValue = enterVerificationCodeDialog.DataContext?.GetType()
                            .GetProperty("SecondField")?.GetValue(enterVerificationCodeDialog.DataContext) as string;
                    }
                    else if (currentDialog is EnterFieldDialog enterFieldDialog)
                    {
                        mainFieldValue = enterFieldDialog.DataContext?.GetType()
                            .GetProperty("MainField")?.GetValue(enterFieldDialog.DataContext) as string;
                        secondFieldValue = enterFieldDialog.DataContext?.GetType()
                            .GetProperty("SecondField")?.GetValue(enterFieldDialog.DataContext) as string;

                    }
                    else if (currentDialog is EnterRepeatedFieldDialog enterRepeatedFieldDialog)
                    {
                        mainFieldValue = enterRepeatedFieldDialog.DataContext?.GetType()
                            .GetProperty("MainField")?.GetValue(enterRepeatedFieldDialog.DataContext) as string;
                        secondFieldValue = enterRepeatedFieldDialog.DataContext?.GetType()
                            .GetProperty("SecondField")?.GetValue(enterRepeatedFieldDialog.DataContext) as string;
                    }

                    if (onSubmitDouble != null)
                    {
                        onSubmitDouble?.Invoke(mainFieldValue, secondFieldValue);
                        CloseDialog();
                    }
                    if (onSubmit != null)
                    {
                        onSubmit?.Invoke(mainFieldValue);
                        CloseDialog();
                    }


                },
                onResendButtonClick: async () =>
                {
                    if (onSecondSubmit != null)
                    {
                        onSecondSubmit?.Invoke(string.Empty);
                        CloseDialog();
                    }
                }) // Invoke the resendAction if provided
            {
                HeaderText = headerText,
                MainFieldWatermarkText = fieldHelperText,
                SubmitButtonText = "Submit",
                ButtonText = "Close"
            };
        }

        public void CloseDialog()
        {
            _mainWindowViewModel.CloseDialog();
        }

    }
}
