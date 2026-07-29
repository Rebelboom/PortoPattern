// ****************************************************************************
// File: ThemeService.cs
// Description: Сервис смены темы WinUI 3.
// Меняет тему XAML-контента и системного TitleBar через AppWindow API.
// ****************************************************************************

#nullable enable

using System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PortoPattern.Interfaces;

namespace PortoPattern.Themes;

public sealed class ThemeService : IThemeService
{
    private readonly IWindowProvider _windowProvider;

    private FrameworkElement? _rootElement;


    public ThemeService(IWindowProvider windowProvider)
    {
        _windowProvider = windowProvider;
    }


    public void SetRootElement(FrameworkElement rootElement)
    {
        ArgumentNullException.ThrowIfNull(rootElement);

        _rootElement = rootElement;
    }


    public void SetTheme(bool isDarkMode)
    {
        ApplyTitleBarTheme(isDarkMode);

        ApplyXamlTheme(isDarkMode);
    }


    private void ApplyTitleBarTheme(bool isDarkMode)
    {
        if (!_windowProvider.IsInitialized)
            return;


        IntPtr hwnd = _windowProvider.GetMainWindowHandle();

        if (hwnd == IntPtr.Zero)
            return;


        var windowId =
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);


        var appWindow =
            AppWindow.GetFromWindowId(windowId);


        if (appWindow == null)
            return;


        appWindow.TitleBar.PreferredTheme =
            isDarkMode
                ? TitleBarTheme.Dark
                : TitleBarTheme.Light;
    }


    private void ApplyXamlTheme(bool isDarkMode)
    {
        if (_rootElement == null)
            return;


        _rootElement.RequestedTheme =
            isDarkMode
                ? ElementTheme.Dark
                : ElementTheme.Light;
    }
}