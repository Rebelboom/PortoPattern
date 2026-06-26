// Файл: Helpers/ProfileUIHelper.cs
// Описание: Вспомогательный класс для управления визуальным состоянием элементов интерфейса.

using Microsoft.UI.Xaml;

namespace PortoPattern.Helpers;

public static class ProfileUIHelper
{
    // Метод для инверсии видимости. 
    // Используется для скрытия корзины у системных профилей.
    // Если isSystem == true, возвращает Collapsed (скрыто), иначе Visible.
    public static Visibility HideIfSystem(bool isSystem)
    {
        return isSystem ? Visibility.Collapsed : Visibility.Visible;
    }

    // Метод для отображения предупреждающих элементов (например, треугольников).
    // Возвращает Visible, если условие истинно.
    public static Visibility ShowIfWarning(bool showWarning)
    {
        return showWarning ? Visibility.Visible : Visibility.Collapsed;
    }
}