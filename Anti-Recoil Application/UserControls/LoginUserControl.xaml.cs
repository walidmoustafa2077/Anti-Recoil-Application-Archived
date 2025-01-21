using Anti_Recoil_Application.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Anti_Recoil_Application.UserControls
{
    /// <summary>
    /// Interaction logic for LoginUserControl.xaml
    /// </summary>
    public partial class LoginUserControl : UserControl
    {
        public LoginUserControl()
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
                var viewModel = DataContext as LoginViewModel;
                if (viewModel != null)
                {
                    // Update the Password property in the ViewModel
                    viewModel.Password = passwordBox.Password;
                }
            }
        }

    }
}
