using Anti_Recoil_Application.ViewModels.DialogViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Anti_Recoil_Application.UserControls.Dialogs
{
    /// <summary>
    /// Interaction logic for EnterUsernameDialog.xaml
    /// </summary>
    public partial class EnterRepeatedFieldDialog : UserControl
    {
        public EnterRepeatedFieldDialog()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Ensure sender is a PasswordBox control
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                // Assuming DataContext is set to the ViewModel
                var viewModel = DataContext as EnterFieldDialogViewModel;
                if (viewModel != null)
                {
                    // Update the Password property in the ViewModel
                    if (passwordBox == Password)
                        viewModel.MainField = passwordBox.Password;
                    else
                        viewModel.SecondField = passwordBox.Password;
                }
            }
        }
    }
}
