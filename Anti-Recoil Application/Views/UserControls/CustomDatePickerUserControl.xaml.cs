using System.Windows;
using System.Windows.Controls;

namespace Anti_Recoil_Application.UserControls
{
    /// <summary>
    /// Interaction logic for CustomDatePickerUserControl.xaml
    /// </summary>
    public partial class CustomDatePickerUserControl : UserControl
    {
        // Dependency Property for Popup State
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(
                nameof(IsPopupOpen),
                typeof(bool),
                typeof(CustomDatePickerUserControl),
                new PropertyMetadata(false));

        // Dependency Property for Selected Date
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
                nameof(SelectedDate),
                typeof(DateTime?),
                typeof(CustomDatePickerUserControl),
                new PropertyMetadata(null));

        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }


        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public CustomDatePickerUserControl()
        {
            InitializeComponent();
        }

        // Toggle Popup on TextBox Click
        private void DateTextBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DatePopup.IsOpen = !DatePopup.IsOpen; // Toggle the popup
        }

        // Handle Date Selection
        private void CalendarControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarControl.SelectedDate.HasValue)
            {
                DateTextBox.Content = CalendarControl.SelectedDate.Value.ToString("dd/MM/yyyy");
                SelectedDate = CalendarControl.SelectedDate;  // Update the SelectedDate property
                DatePopup.IsOpen = false; // Close the popup after selection
            }
        }
    }
}
