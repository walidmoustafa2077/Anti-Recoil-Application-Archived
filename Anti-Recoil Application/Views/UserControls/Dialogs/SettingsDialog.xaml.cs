using System.Windows.Controls;
using System.Windows.Input;

namespace Anti_Recoil_Application.UserControls.Dialogs
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : UserControl
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 75);
            }
            else
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 75);
            }
            e.Handled = true;
        }

        private void Slider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the position of the mouse relative to the slider
            var slider = sender as Slider;
            var mousePos = e.GetPosition(slider);

            // Calculate the new value based on the mouse position
            double newValue = (mousePos.X / slider.ActualWidth) * (slider.Maximum - slider.Minimum) + slider.Minimum;

            // Set the slider value to the new calculated value
            slider.Value = newValue;
        }

    }
}
