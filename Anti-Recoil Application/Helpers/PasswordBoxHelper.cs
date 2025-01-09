namespace Anti_Recoil_Application.Helpers
{
    using System.Windows;
    using System.Windows.Controls;

    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty IsPasswordEmptyProperty =
            DependencyProperty.RegisterAttached(
                "IsPasswordEmpty",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(true));

        public static bool GetIsPasswordEmpty(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPasswordEmptyProperty);
        }

        public static void SetIsPasswordEmpty(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPasswordEmptyProperty, value);
        }

        public static readonly DependencyProperty MonitorPasswordProperty =
            DependencyProperty.RegisterAttached(
                "MonitorPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnMonitorPasswordChanged));

        public static bool GetMonitorPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(MonitorPasswordProperty);
        }

        public static void SetMonitorPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(MonitorPasswordProperty, value);
        }

        private static void OnMonitorPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                    UpdateIsPasswordEmpty(passwordBox);
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                UpdateIsPasswordEmpty(passwordBox);
            }
        }

        private static void UpdateIsPasswordEmpty(PasswordBox passwordBox)
        {
            SetIsPasswordEmpty(passwordBox, string.IsNullOrEmpty(passwordBox.Password));
        }
    }

}
