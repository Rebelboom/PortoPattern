// ****************************************************************************
// File: IThemeService.cs
// Description: Интерфейс сервиса переключения тем.
// ****************************************************************************

#nullable enable

using Microsoft.UI.Xaml;

namespace PortoPattern.Themes;

public interface IThemeService
{
    void SetRootElement(FrameworkElement rootElement);

    void SetTheme(bool isDarkMode);
}