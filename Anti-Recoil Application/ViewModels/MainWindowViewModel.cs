using Anti_Recoil_Application.ViewModels.DialogViewModels;

namespace Anti_Recoil_Application.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private object _currentDialog;

        private bool _isDialogVisible;
        private bool _isLoading;

        public object CurrentDialog
        {
            get => _currentDialog;
            set => SetProperty(ref _currentDialog, value);
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

        public void ShowDialog(object dialogViewModel, bool isRetyped)
        {
            // Ensure that the dialogViewModel is set
            //CurrentDialog = dialogViewModel;
            //IsDialogVisible = true;
            if (dialogViewModel is EnterFieldDialogViewModel enterUsernameDialog)
            {
                if (isRetyped)
                {
                    CurrentDialog = new UserControls.Dialogs.EnterRepeatedFieldDialog { DataContext = enterUsernameDialog };
                    IsDialogVisible = true;
                }
                else
                {
                    CurrentDialog = new UserControls.Dialogs.EnterFieldDialog { DataContext = enterUsernameDialog };
                    IsDialogVisible = true;
                }
            }
            else if (dialogViewModel is MainDialogViewModel dialog)
            {
                CurrentDialog = new UserControls.Dialogs.MainDialog { DataContext = dialog };
                IsDialogVisible = true;
            }

            IsDialogVisible = true;
        }

        public void CloseDialog()
        {
            // Ensure that the dialogViewModel is cleared
            CurrentDialog = null;
            IsDialogVisible = false;
        }
    }
}
