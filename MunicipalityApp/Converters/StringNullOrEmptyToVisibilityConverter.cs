using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MunicipalityApp.Converters
{

    // Converts a string value
    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        /// Converts a string to visibility based on null/empty state.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            return string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
        }


        // Not supported
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
