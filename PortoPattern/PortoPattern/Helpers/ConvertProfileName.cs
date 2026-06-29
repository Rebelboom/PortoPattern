using Microsoft.UI.Xaml.Data;
using System;
using Windows.UI.Xaml.Data; // Используйте Microsoft.UI.Xaml.Data, если это WinUI 3

namespace PortoPattern.Helpers
{
    // Конвертер для преобразования системных имен профилей в удобный для чтения формат
    public class ConvertProfileName : IValueConverter
    {
        // Выполняет преобразование исходного значения перед выводом в UI
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // Проверяем, что входящее значение не пустое и является строкой
                if (value is string profileName)
                {
                    // Форматируем или переводим системные имена
                    switch (profileName)
                    {
                        case "System":
                        case "SystemProfile":
                            return "Системный профиль";
                        case "Default":
                        case "DefaultProfile":
                            return "По умолчанию";
                        // Если имя не системное, возвращаем его как есть для пользовательских профилей
                        default:
                            return profileName;
                    }
                }

                // Возвращаем оригинальное значение, если преобразование неприменимо
                return value;
            }
            catch (Exception ex)
            {
                // Выводим ошибки в консоль только при отладке, исключая их из релизной сборки
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"ConvertProfileName Error: {ex.Message}");
#endif
                return value;
            }
        }

        // Обратное преобразование не требуется, так как привязка односторонняя (OneTime/OneWay)
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}