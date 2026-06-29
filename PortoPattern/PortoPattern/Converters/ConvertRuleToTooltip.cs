using System;
using System.IO;
using Microsoft.UI.Xaml.Data;
using PortoPattern.Core.IgnorSpace;

namespace PortoPattern.Converters
{
    // Конвертер для создания всплывающей подсказки (ToolTip) на основе режима правила фильтрации.
    public class ConvertRuleToTooltip : IValueConverter
    {
        // Преобразует объект IgnorRule в строку для ToolTip в зависимости от значения свойства Mode.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IgnorRule rule)
            {
                try
                {
                    // Проверяем режим правила для формирования текста подсказки.
                    // Логика применяется только для пользовательских профилей, 
                    // но проверка внутри конвертера гарантирует безопасность.
                    if (rule.Mode.ToString() == "GlobalName")
                    {
                        // В глобальном режиме извлекаем только имя конечной папки.
                        string folderName = Path.GetFileName(rule.RealPath.TrimEnd('\\', '/')) ?? rule.RealPath;
                        return $"все директории {folderName}";
                    }
                    else if (rule.Mode.ToString() == "ExactPath")
                    {
                        // В режиме точного пути возвращаем полный маршрут.
                        return $"полный путь {rule.RealPath}";
                    }
                }
                catch (Exception ex)
                {
                    // Ошибки выводим в консоль только при отладке, чтобы не ломать релизную версию.
#if DEBUG
                    Console.WriteLine($"Ошибка в ConvertRuleToTooltip: {ex.Message}");
#endif
                    // В случае непредвиденного сбоя возвращаем оригинальный путь как страховку.
                    return rule.RealPath ?? string.Empty;
                }
            }

            // Возвращаем пустую строку, если значение null или тип не совпадает.
            return string.Empty;
        }

        // Обратное преобразование не требуется для ToolTip (работает только OneWay).
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}