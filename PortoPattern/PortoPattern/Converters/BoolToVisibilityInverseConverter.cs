// ****************************************************************************
// Файл: BoolToVisibilityInverseConverter.cs
// Описание: Инвертированный логический конвертер.
//
// ПРИМЕНЕНИЕ:
// - true -> Collapsed (скрыть, если условие выполнено).
// - false -> Visible (показать, если условие не выполнено).
// Идеален для элементов-заглушек и сообщений об отсутствии данных.
// ****************************************************************************

#nullable enable

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PortoPattern.Converters;

/// <summary>
/// Скрывает элемент, если логическое значение истинно.
/// </summary>
public class BoolToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b)
        {
            return b ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility vis)
        {
            return vis != Visibility.Visible;
        }
        return true;
    }
}