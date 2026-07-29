#nullable enable
// File: ColorManager.cs

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace PortoPattern.Themes;

public static class ColorManager
{
    public static float BackdropTintOpacity { get; set; } = 0.5f;

    public static Color GetSurfaceColor()
    {
        if (Application.Current.Resources.TryGetValue("Porto.Surface.Root", out var resource))
        {
            if (resource is SolidColorBrush brush)
            {
                return brush.Color;
            }
            if (resource is Color color)
            {
                return color;
            }
        }

        return Color.FromArgb(0, 0, 0, 0);
    }
}