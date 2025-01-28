using Anti_Recoil_Application.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            // Attach GotFocus handler to TextBox


            // Attach GotFocus event to all TextBox controls in the container
            foreach (var child in MyContainer.Children)
            {
                if (child is TextBox textBox)
                {
                    textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
                    textBox.GotFocus += TextBox_GotFocus;
                }
                else if (child is PasswordBox passwordBox)
                {
                    passwordBox.PreviewMouseDown += PasswordBox_PreviewMouseDown;
                    passwordBox.GotFocus += PasswordBox_GotFocus;
                }
            }
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

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                // Prevent the mouse click from deselecting text
                e.Handled = true;
                textBox.Focus();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                // Use a dispatcher to delay the SelectAll call until after the MouseDown event
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    textBox.SelectAll();
                }));
            }
        }

        private void PasswordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PasswordBox passwordBox && !passwordBox.IsKeyboardFocusWithin)
            {
                // Prevent the mouse click from deselecting text
                e.Handled = true;
                passwordBox.Focus();
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && !string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                // Programmatically select all the text in the PasswordBox
                passwordBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    passwordBox.SelectAll();
                }));
            }
        }
    }
}
