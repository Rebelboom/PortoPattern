// Файл: ColorManager.cs
// Описание: Отвечает за безопасное извлечение цветов из App.xaml и настройку прозрачности фона.
// Правки: Новый класс, заменяющий ThemePalette и ThemeManager.

#nullable enable
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace PortoPattern.Themes;

public static class ColorManager
{
    /// <summary>
    /// Глобальная настройка прозрачности для Acrylic-эффекта.
    /// Можно менять в рантайме, если потребуется настройка.
    /// </summary>
    public static float BackdropTintOpacity { get; set; } = 0.5f;

    /// <summary>
    /// Извлекает базовый цвет поверхности из глобальных ресурсов (App.Surface).
    /// </summary>
    public static Color GetSurfaceColor()
    {
        // Пытаемся найти ресурс по ключу
        if (Application.Current.Resources.TryGetValue("Porto.Surface.Root", out var resource))
        {
            // WinUI хранит цвета в SolidColorBrush, извлекаем структуру Color
            if (resource is SolidColorBrush brush)
            {
                return brush.Color;
            }
            if (resource is Color color)
            {
                return color;
            }
        }

#if DEBUG
        // Выводим ошибку только при отладке, если ключ не найден или тип не совпадает
        System.Diagnostics.Debug.WriteLine("[DEBUG ERROR] ColorManager: Resource 'App.Surface' not found or invalid format. Returning transparent fallback.");
#endif

        // Безопасный резервный цвет (прозрачный), если ресурс отсутствует
        return Color.FromArgb(0, 0, 0, 0);
    }
}