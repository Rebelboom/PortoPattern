// ****************************************************************************
// File: MainWindow.xaml.cs
// Description: Главное окно приложения. Управляет контейнером, HWND и сохранением 
// акрила при потере фокуса прямо в бэхайнде без изменения BackdropManager.
// ****************************************************************************

#nullable enable

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using PortoPattern.Interfaces;
using PortoPattern.Services;
using PortoPattern.Themes;
using PortoPattern.Views;
using System;
using System.Reflection;
using WinRT.Interop;

namespace PortoPattern;

/// <summary>
/// Главное окно приложения WinUI 3.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly BackdropManager _backdropManager;

    /// <summary>
    /// Инициализирует новый экземпляр класса MainWindow.
    /// </summary>
    public MainWindow(ShellPage shellPage, IWindowProvider windowProvider)
    {
        try
        {
            // Инициализация графических компонентов XAML интерфейса
            this.InitializeComponent();

            // Получение дескриптора текущего окна (HWND) для системных интеграций
            IntPtr hwnd = WindowNative.GetWindowHandle(this);

            if (hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("Не удалось получить HWND для MainWindow.");
            }

            // Передача HWND в провайдер для доступа из сервисов
            if (windowProvider is WindowProvider wp)
            {
                wp.SetWindowHandle(hwnd);
            }

            // Добавление корневой страницы ShellPage в контейнер окна
            if (RootContainer != null)
            {
                RootContainer.Children.Add(shellPage);
            }

            // Расширение содержимого на область системного заголовка окна
            ExtendsContentIntoTitleBar = true;

            // Инициализация менеджера системных подложек (акрила)
            _backdropManager = new BackdropManager(this);
            _backdropManager.Initialize();

            // Подписка на событие изменения активности окна для удержания акрила при потере фокуса
            this.Activated += OnWindowActivated;

        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод ошибок инициализации главного окна в консоль отладки
            Console.WriteLine($"[DEBUG ERROR] MainWindow.Constructor: {ex.Message}");
            if (ex.StackTrace != null)
            {
                Console.WriteLine(ex.StackTrace);
            }
#endif
            throw;
        }
    }

    /// <summary>
    /// Обработчик события изменения активности окна. Предотвращает затухание акрила 
    /// при клике за пределами приложения без прикосновения к классу BackdropManager.
    /// </summary>
    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        try
        {
            // При изменении состояния фокуса принудительно оставляем подложку активной,
            // чтобы Windows не заменяла полупрозрачный акрил на сплошной цвет.
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                ForceKeepAcrylicActive();
            }
            else
            {
                ForceKeepAcrylicActive();
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод ошибок обработки фокуса окна в консоль отладки
            Console.WriteLine($"[DEBUG ERROR] MainWindow.OnWindowActivated: {ex.Message}");
#endif
        }
    }

    /// <summary>
    /// Принудительно устанавливает флаг IsInputActive в конфигурации BackdropManager через рефлексию,
    /// позволяя управлять эффектом из бэхаунда без изменения внешних файлов.
    /// </summary>
    private void ForceKeepAcrylicActive()
    {
        try
        {
            // Получаем приватное поле конфигурации из экземпляра BackdropManager
            FieldInfo? configField = _backdropManager.GetType().GetField(
                "_configurationSource",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (configField?.GetValue(_backdropManager) is SystemBackdropConfiguration config)
            {
                config.IsInputActive = true;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод ошибок рефлексии в консоль отладки
            Console.WriteLine($"[DEBUG ERROR] MainWindow.ForceKeepAcrylicActive: {ex.Message}");
#endif
        }
    }
}