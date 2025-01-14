using System.Windows;

namespace Anti_Recoil_Application.Helpers
{
    public static class WatermarkHelper
    {
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.RegisterAttached(
                "WatermarkText",
                typeof(string),
                typeof(WatermarkHelper),
                new PropertyMetadata(string.Empty));

        public static void SetWatermarkText(UIElement element, string value)
        {
            element.SetValue(WatermarkTextProperty, value);
        }

        public static string GetWatermarkText(UIElement element)
        {
            return (string)element.GetValue(WatermarkTextProperty);
        }
    }
}
