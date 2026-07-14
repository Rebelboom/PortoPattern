#nullable enable

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using PortoPattern.Core.IgnorNew;

namespace PortoPattern.Converters;

public sealed class AdminRequiredVisibilityConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        if (value is bool requiresAdmin)
        {
            // Показываем только когда правило требует администратора
            // и приложение запущено НЕ от администратора.
            bool showWarning =
                requiresAdmin &&
                !IgnoreItem.IsUserAdministrator();

            return showWarning
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }


    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        throw new NotSupportedException();
    }
}