using System;
using System.Globalization;
using System.Windows.Data;

namespace SupernoteDesktopClient.Converters
{
    internal class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not String enumString)
                throw new ArgumentException("Parameter must be enumeration");

            if (Enum.IsDefined(typeof(Wpf.Ui.Appearance.ThemeType), value) == false)
                throw new ArgumentException("Parameter must be enumeration of ThemeType");

            var enumValue = Enum.Parse(typeof(Wpf.Ui.Appearance.ThemeType), enumString);

            return enumValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not String enumString)
                throw new ArgumentException("Parameter must be enumeration");

            return Enum.Parse(typeof(Wpf.Ui.Appearance.ThemeType), enumString);
        }
    }
}
