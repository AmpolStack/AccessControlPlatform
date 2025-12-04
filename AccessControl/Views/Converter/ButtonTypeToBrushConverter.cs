using AccessControl.Views.Enums;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AccessControl.Views.Converter
{
    public class ButtonTypeToBrushConverter : IValueConverter
    {
        public ButtonTypeToBrushConverter() { }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AppElementType buttonType)
            {
                return buttonType switch
                {
                    AppElementType.Sober => GetBrush("AppBlackBrush"),
                    AppElementType.Vibrant => GetBrush("AppBlueBrush"),
                    AppElementType.Success => GetBrush("AppGreenBrush"),
                    AppElementType.Danger => GetBrush("AppRedBrush"),

                    _ => GetBrush("AppBlackBrush")
                };
            }

            return GetBrush("AppBlackBrush");
        }

        private static object GetBrush(string key)
            => (Brush)Application.Current.FindResource(key);


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
