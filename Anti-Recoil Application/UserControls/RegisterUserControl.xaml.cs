using Anti_Recoil_Application.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Anti_Recoil_Application.UserControls
{
    /// <summary>
    /// Interaction logic for RegisterUserControl.xaml
    /// </summary>
    public partial class RegisterUserControl : UserControl
    {
        public RegisterUserControl()
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
                var viewModel = DataContext as RegisterViewModel;
                if (viewModel != null)
                {
                    // Update the Password property in the ViewModel
                    // Check which PasswordBox triggered the event
                    if (passwordBox.Name == "PasswordBox")  // Name of the first PasswordBox (Password)
                    {
                        // Update the Password property in the ViewModel
                        viewModel.Password = passwordBox.Password;
                    }
                    else if (passwordBox.Name == "ConfirmPasswordBox")  // Name of the second PasswordBox (Confirm Password)
                    {
                        // Update the ConfirmPassword property in the ViewModel
                        viewModel.ConfirmPassword = passwordBox.Password;
                    }
                }
            }
        }
    }
}
