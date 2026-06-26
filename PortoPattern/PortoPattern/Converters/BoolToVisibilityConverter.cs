// ****************************************************************************
// Файл: BoolToVisibilityConverter.cs
// Описание: Универсальный конвертер логических состояний в видимость WinUI.
//
// НАЗНАЧЕНИЕ:
// - Преобразование true -> Visible, false -> Collapsed.
// - Поддержка инверсии логики через передачу любого строкового параметра 
//   в ConverterParameter (например, ConverterParameter=Invert).
// ****************************************************************************

#nullable enable

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PortoPattern.Converters;

/// <summary>
/// Конвертирует логическое значение в перечисление Visibility.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    // =========================================================================
    // ЛОГИКА ПРЕОБРАЗОВАНИЯ (Convert)
    // =========================================================================

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b)
        {
            // Если в XAML задан параметр, инвертируем результат
            bool invert = parameter != null;
            if (invert) return b ? Visibility.Collapsed : Visibility.Visible;

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    // =========================================================================
    // ОБРАТНОЕ ПРЕОБРАЗОВАНИЕ (ConvertBack)
    // =========================================================================

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility vis)
        {
            return vis == Visibility.Visible;
        }
        return false;
    }
}