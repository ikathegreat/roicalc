using System;
using System.Globalization;
using System.Windows.Data;

namespace SNROI
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = value != null && (bool)value;
            return !output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = value != null && (bool)value;
            return !output;
        }

        #endregion
    }

}
