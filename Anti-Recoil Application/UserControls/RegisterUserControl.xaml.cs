using Anti_Recoil_Application.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Anti_Recoil_Application.UserControls
{
    public partial class RegisterUserControl : UserControl
    {
        public RegisterUserControl()
        {
            InitializeComponent();

            // Attach GotFocus and PreviewMouseDown events to TextBox and PasswordBox controls
            AttachEventHandlersToControls(MyContainer);
            AttachEventHandlersToControls(MyContainer2);
            AttachEventHandlersToControls(MyContainer3);
        }

        // Method to attach event handlers to TextBox and PasswordBox controls in a given container
        private void AttachEventHandlersToControls(Panel container)
        {
            foreach (var child in container.Children)
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
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                var viewModel = DataContext as RegisterViewModel;
                if (viewModel != null)
                {
                    if (passwordBox.Name == "PasswordBox")
                    {
                        viewModel.Password = passwordBox.Password;
                    }
                    else if (passwordBox.Name == "ConfirmPasswordBox")
                    {
                        viewModel.ConfirmPassword = passwordBox.Password;
                    }
                }
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                textBox.Focus();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
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
                e.Handled = true;
                passwordBox.Focus();
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && !string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    passwordBox.SelectAll();
                }));
            }
        }
    }
}
