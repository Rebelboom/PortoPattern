// ****************************************************************************
// Файл: NullToVisibilityConverter.cs
// Описание: Конвертер состояний наличия данных (Presence Converter).
//
// НАЗНАЧЕНИЕ:
// - null -> Collapsed (скрыть пустой блок).
// - not null -> Visible (отобразить данные).
// - Поддержка гибкой инверсии через ConverterParameter.
// ****************************************************************************

#nullable enable

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PortoPattern.Converters;

/// <summary>
/// Управляет видимостью элемента в зависимости от наличия привязанного объекта.
/// </summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    // =========================================================================
    // ОСНОВНАЯ ЛОГИКА
    // =========================================================================

    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        bool invert = parameter != null;
        bool isNull = value == null;

        if (invert)
        {
            // Показать, если данных НЕТ
            return isNull ? Visibility.Visible : Visibility.Collapsed;
        }

        // Показать, если данные ЕСТЬ
        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // Обратная конвертация из состояния видимости в абстрактный объект невозможна.
        throw new NotSupportedException("NullToVisibilityConverter: ConvertBack не поддерживается.");
    }

    // TODO: В будущем добавить проверку на пустые строки или коллекции (IEnumerable.Any).
}