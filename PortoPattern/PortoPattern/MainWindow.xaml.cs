#nullable enable

// File: MainWindow.xaml.cs
// Description: Главное окно приложения. Отвечает за инстанцирование ShellPage, 
// настройку кастомного заголовка и управление системными материалами (Backdrop).
// Правки: Полностью удалена зависимость от убранного ThemeService.

using System;
using Microsoft.UI.Xaml;
using PortoPattern.Views;
using PortoPattern.Themes;
using PortoPattern.Interfaces;
using PortoPattern.Services;
using WinRT.Interop;

namespace PortoPattern;

/// <summary>
/// Главное окно приложения WinUI 3.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly BackdropManager _backdropManager;

    // ПРАВКА: Из параметров конструктора удален ThemeService
    public MainWindow(ShellPage shellPage, IWindowProvider windowProvider)
    {
        try
        {
            this.InitializeComponent();

            // Получение HWND (дескриптора окна) для интеграции с низкоуровневыми API Windows
            IntPtr hwnd = WindowNative.GetWindowHandle(this);

            if (hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("Не удалось получить HWND для MainWindow.");
            }

            // Инициализация провайдера дескриптора окна для сервисов диалогов и навигации
            if (windowProvider is WindowProvider wp)
            {
                wp.SetWindowHandle(hwnd);
            }

            // Установка основного контента (ShellPage) в контейнер окна
            if (RootContainer != null)
            {
                RootContainer.Children.Add(shellPage);
            }

            // Расширение контента на область заголовка для создания кастомного дизайна
            ExtendsContentIntoTitleBar = true;

            // ПРАВКА: Инициализация BackdropManager теперь выполняется без передачи удаленного ThemeService
            _backdropManager = new BackdropManager(this);
            _backdropManager.Initialize();
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод ошибок инициализации окна в консоль отладки только в DEBUG конфигурации
            Console.WriteLine($"[DEBUG ERROR] MainWindow.Constructor: {ex.Message}");
            if (ex.StackTrace != null)
            {
                Console.WriteLine(ex.StackTrace);
            }
#endif
            throw;
        }
    }

    // TODO: Инкапсулировать логику приведения типов внутри DI или фабрики, чтобы избежать 'is WindowProvider' в UI-слое.
}