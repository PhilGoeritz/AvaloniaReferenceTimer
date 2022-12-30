using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;

namespace ReferenceTimer.Logic
{
    internal class PathToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is not string rawUri || !targetType.IsAssignableFrom(typeof(Bitmap)))
                throw new NotSupportedException();

            if (string.IsNullOrEmpty(rawUri))
                return null;

            var uri = new Uri(rawUri);

            return new Bitmap(uri.AbsolutePath);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
