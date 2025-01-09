using Anti_Recoil_Application.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anti_Recoil_Application.UserControls
{
    /// <summary>
    /// Interaction logic for HomeUserControl.xaml
    /// </summary>
    public partial class HomeUserControl : UserControl
    {

        public HomeUserControl()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null && e.Delta != 0)
            {
                var scrollViewer = FindScrollViewer(listView);
                if (scrollViewer != null)
                {
                    // Get the current horizontal offset and the width of one item
                    double currentOffset = scrollViewer.HorizontalOffset;
                    double maxOffset = scrollViewer.ScrollableWidth;
                    double itemWidth = 100; // Assuming each item has a fixed width, you can dynamically set this based on item size.

                    // Calculate the new offset based on the scroll direction and sensitivity
                    double newHorizontalOffset = currentOffset;

                    if (e.Delta < 0) // Scroll Down (positive Delta)
                    {
                        newHorizontalOffset = currentOffset + itemWidth; // Move one item
                        if (newHorizontalOffset > maxOffset)
                        {
                            newHorizontalOffset = maxOffset;
                        }
                    }
                    else if (e.Delta > 0) // Scroll Up (negative Delta)
                    {
                        newHorizontalOffset = currentOffset - 3 * itemWidth; // Skip 3 items
                        if (newHorizontalOffset < 0)
                        {
                            newHorizontalOffset = 0; // Prevent scrolling beyond the start
                        }
                    }

                    // Apply the new horizontal offset
                    scrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
                }
            }
            e.Handled = true;
        }



        private ScrollViewer FindScrollViewer(DependencyObject depObj)
        {
            if (depObj == null) return null;
            if (depObj is ScrollViewer) return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = FindScrollViewer(child);
                if (result != null) return result;
            }

            return null;
        }


    }
}
