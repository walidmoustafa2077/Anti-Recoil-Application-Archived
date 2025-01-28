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


        public async Task ShowDialogAsync(string message)
        {
            var dialogViewModel = new MainDialogViewModel(() => CloseDialog())
            {
                HeaderText = message,
                ButtonText = "Close"
            };

            await _mainWindowViewModel.ShowDialogAsync(dialogViewModel);
        }


        public EnterFieldDialogViewModel CreateEnterFieldDialogViewModel(string headerText, string mainField, string fieldHelperText, Action<string>? onSubmit)
        {
            return new EnterFieldDialogViewModel(
                onButtonClick: () => CloseDialog(),
                onSubmitButtonClick: () =>
                {
                    string? mainFieldValue = null;  // Declare the variable outside of the if block
                    var currentDialog = _mainWindowViewModel.CurrentDialog;

                    // Check if currentDialog is of type EnterFieldDialog or EnterRepeatedFieldDialog
                    if (currentDialog is EnterFieldDialog enterFieldDialog)
                    {
                        mainFieldValue = enterFieldDialog.DataContext?.GetType()
                            .GetProperty("MainField")?.GetValue(enterFieldDialog.DataContext) as string;
                    }
                    else if (currentDialog is EnterRepeatedFieldDialog enterRepeatedFieldDialog)
                    {
                        mainFieldValue = enterRepeatedFieldDialog.DataContext?.GetType()
                            .GetProperty("MainField")?.GetValue(enterRepeatedFieldDialog.DataContext) as string;
                    }
                    CloseDialog();

                    // Check if mainFieldValue is not null before invoking onSubmit
                    if (mainFieldValue != null)
                        onSubmit?.Invoke(mainFieldValue);


                })
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
