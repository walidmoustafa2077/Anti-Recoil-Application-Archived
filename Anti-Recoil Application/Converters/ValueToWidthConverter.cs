using System.Globalization;
using System.Windows.Data;

namespace Anti_Recoil_Application.Converters
{
    public class ValueToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure that values contain both slider value and track width.
            if (values != null && values.Length == 2)
            {
                double sliderValue = (double)values[0];  // Slider Value
                double sliderWidth = (double)values[1];  // Actual Width of the track (TrackBackground)

                // You can get the Min and Max dynamically from the Slider if needed
                double sliderMin = 50; // Default minimum value of the slider
                double sliderMax = 140; // Default maximum value of the slider

                // Scale the value relative to the range (sliderMax - sliderMin) and multiply by the width of the slider
                if (sliderMax != sliderMin)
                {
                    return (sliderValue - sliderMin) / (sliderMax - sliderMin) * sliderWidth;
                }
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
