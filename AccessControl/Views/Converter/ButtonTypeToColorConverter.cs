using AccessControl.Views.Enums;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AccessControl.Views.Converter
{
    class ButtonTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is AppElementType buttonType)
            {
                return buttonType switch
                {
                    AppElementType.Sober => GetColor("AppBlackColor"),
                    AppElementType.Vibrant => GetColor("AppBlueColor"),
                    AppElementType.Success => GetColor("AppGreenColor"),
                    AppElementType.Danger => GetColor("AppRedColor"),

                    _ => GetColor("AppBlackColor")
                };
            }

            return GetColor("AppBlackColor");
        }

        private static object GetColor(string key)
           => Application.Current.FindResource(key);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
