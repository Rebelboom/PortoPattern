using System;
using Microsoft.UI.Xaml.Data;
using System.IO;

namespace PortoPattern.Converters
{
    public class ConvertProfileName : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, string language)
        {
            if (value == null) return string.Empty;

            string path = value.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            try
            {
                // Извлекаем только имя файла/папки из полного пути
                return Path.GetFileName(path.TrimEnd('\\', '/')) ?? path;
            }
            catch
            {
                return path;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}