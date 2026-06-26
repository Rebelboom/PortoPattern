// ****************************************************************************
// Файл: InverseNullToVisibilityConverter.cs
// Описание: Инвертированный детектор наличия данных.
//
// НАЗНАЧЕНИЕ:
// - null -> Visible (показать состояние ожидания или пустоты).
// - not null -> Collapsed (скрыть, когда данные загружены).
// ****************************************************************************

#nullable enable

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PortoPattern.Converters;

/// <summary>
/// Отображает элемент только в том случае, если привязанное значение отсутствует.
/// </summary>
public sealed class InverseNullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        // Если объекта нет — показываем элемент (заглушку)
        return value == null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException("InverseNullToVisibilityConverter: ConvertBack не поддерживается.");
    }
}