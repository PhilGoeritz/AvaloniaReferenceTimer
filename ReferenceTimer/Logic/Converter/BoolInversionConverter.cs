using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ReferenceTimer.Logic.Converter
{
    internal class BoolInversionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isTrue
                ? !isTrue
                : null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool isTrue
                ? !isTrue
                : null;
        }
    }
}
