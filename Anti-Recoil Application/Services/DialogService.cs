using Anti_Recoil_Application.ViewModels;

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

        public void CloseDialog()
        {
            _mainWindowViewModel.CloseDialog();

        }
    }
}
