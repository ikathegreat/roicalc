using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SNROI.Views.Utilities
{
    public class InverseBooleanConverter : IValueConverter
    {
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
    }

    public class FileNameWithoutPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            return Path.GetFileName(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class EnumToIndexConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            return (int)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.ToObject(targetType, (int)value);
        }
    }

    public class ObjectsEqualityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            var result = string.Equals(value.ToString(), parameter);
            return Inverse ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
    }

    public class BoolToStringConverter : BoolToValueConverter<string> { }

    public class BoolToBrushConverter : BoolToValueConverter<Brush> { }

    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { }

    public class BoolToObjectConverter : BoolToValueConverter<object> { }

    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }

        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }

    public class InRangeToBoolConvertor : IValueConverter
    {
        public double Maximum { get; set; }
        public double Minimum { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double data;

            if (!double.TryParse(value.ToString(), out data)) return false;

            if (data >= Minimum && data <= Maximum)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = parameter as string;

            if (!string.IsNullOrEmpty(format))
                return string.Format(culture, format, value);
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class VisiblityToInverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;

            if (visiblity == Visibility.Visible)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SecondsToTimeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double seconds;
                double.TryParse(value.ToString(), out seconds);

                return TimeSpan.FromSeconds(Math.Round(seconds)).ToString("c");
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType,
          object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        public EnumBindingSourceExtension()
        {
        }

        private Type enumType;

        public Type EnumType
        {
            get => enumType;
            set
            {
                if (value == this.enumType)
                    return;
                if (null != value)
                {
                    var underlyingEnumType = Nullable.GetUnderlyingType(value) ?? value;
                    if (!underlyingEnumType.IsEnum)
                        throw new ArgumentException("Type must be an Enum!");
                }

                this.enumType = value;
            }
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this.enumType)
                throw new InvalidOperationException("The EnumType must be specified!");

            var actualEnumType = Nullable.GetUnderlyingType(this.enumType) ?? this.enumType;
            var enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this.enumType)
                return enumValues;

            var tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}