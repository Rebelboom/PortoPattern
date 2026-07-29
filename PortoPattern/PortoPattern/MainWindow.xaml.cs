#nullable enable

// File: MainWindow.xaml.cs

using System;
using Microsoft.UI.Xaml;
using PortoPattern.Views;
using PortoPattern.Themes;
using PortoPattern.Interfaces;
using PortoPattern.Services;
using PortoPattern.Core.Settings;   // === ИЗМЕНЕНО ===
using WinRT.Interop;

namespace PortoPattern;

public sealed partial class MainWindow : Window
{
    private readonly BackdropManager _backdropManager;


    public MainWindow(
        ShellPage shellPage,
        IWindowProvider windowProvider,
        IThemeService themeService,
        ISettingsService settingsService)   // === ИЗМЕНЕНО ===
    {
        InitializeComponent();


        IntPtr hwnd = WindowNative.GetWindowHandle(this);


        if (hwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Не удалось получить HWND для MainWindow.");
        }


        if (windowProvider is WindowProvider wp)
        {
            wp.SetWindowHandle(hwnd);
        }


        ExtendsContentIntoTitleBar = true;


        _backdropManager = new BackdropManager(this);
        _backdropManager.Initialize();


        ContentFrame.Content = shellPage;


        themeService.SetRootElement(ContentFrame);

        // === ИЗМЕНЕНО ===
        // Применяем сохранённую тему сразу после регистрации
        // корневого элемента.
        themeService.SetTheme(settingsService.Settings.IsDarkMode);
    }
}