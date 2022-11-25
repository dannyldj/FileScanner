using System;
using System.Globalization;
using System.Windows.Data;

namespace Scanner.Converters
{
    public class FileSizeToStringConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is long size)
            {
                if (size < 1024)
                {
                    return size.ToString("F0") + "bytes";
                }
                else if ((size >> 10) < 1024)
                {
                    return (size / 1024F).ToString("F0") + "KB";
                }
                else if ((size >> 20) < 1024)
                {
                    return ((size >> 10) / 1024F).ToString("F0") + "MB";
                }
                else if ((size >> 30) < 1024)
                {
                    return ((size >> 20) / 1024F).ToString("F0") + "GB";
                }
                else if ((size >> 40) < 1024)
                {
                    return ((size >> 30) / 1024F).ToString("F0") + "TB";
                }
                else if ((size >> 50) < 1024)
                {
                    return ((size >> 40) / 1024F).ToString("F0") + "PB";
                }
                else
                {
                    return ((size >> 50) / 1024F).ToString("F0") + "EB";
                }
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }
}
