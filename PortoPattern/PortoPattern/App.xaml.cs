#nullable enable

// File: App.xaml.cs
// Description: Точка входа WinUI 3 приложения. Управляет жизненным циклом окна, инициализацией навигации, а также безопасной загрузкой темы.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using PortoPattern.Core.Settings;
using PortoPattern.Core.Themes;
using PortoPattern.DI;

namespace PortoPattern;

/// <summary>
/// Точка входа приложения.
/// Отвечает за:
/// - создание окна
/// - инициализацию DI
/// - загрузку настроек и темы
/// - запуск UI
/// </summary>
public partial class App : Application
{
    private Window? m_window;

    /// <summary>
    /// Инициализирует новый экземпляр класса App.
    /// </summary>
    public App()
    {
        // Инициализация базовых компонентов XAML платформы WinUI 3
        this.InitializeComponent();

#if DEBUG
        // Глобальный перехват UI-исключений с выводом в консоль отладки
        UnhandledException += (sender, e) =>
        {
            e.Handled = true;
            Console.WriteLine($"[DEBUG ERROR] App.xaml.cs: Unhandled UI Exception: {e.Exception?.Message}");

            if (e.Exception?.StackTrace != null)
            {
                Console.WriteLine(e.Exception.StackTrace);
            }

            if (e.Exception?.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {e.Exception.InnerException.Message}");
            }
        };
#endif
    }

    /// <summary>
    /// Вызывается при запуске приложения платформой.
    /// </summary>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            // Получаем DI контейнер из статического свойства AppLauncher
            var provider = AppLauncher.Host?.Services;

            if (provider == null)
            {
                throw new InvalidOperationException("Host не инициализирован. Проверьте последовательность запуска в AppLauncher.");
            }

            // Загружаем настройки и инициализируем тему перед созданием окна, 
            // когда рантайм WinUI 3 и URI-парсер уже полностью готовы к работе.
            LoadSettingsAndTheme(provider);

            // Инициализация маршрутов навигации (Extension method из PortoPattern.DI)
            provider.MapNavigation();

            // Создание главного окна через DI
            m_window = provider.GetRequiredService<MainWindow>();

            // =========================================================
            // Запуск UI
            // =========================================================

            m_window.Activate();
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод критических ошибок запуска в консоль отладки
            Console.WriteLine($"[DEBUG ERROR] App.OnLaunched: {ex.Message}");
            if (ex.StackTrace != null) Console.WriteLine(ex.StackTrace);
#endif
            throw;
        }
    }

    /// <summary>
    /// Безопасно загружает конфигурацию настроек и применяет тему приложения.
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов DI.</param>
    private void LoadSettingsAndTheme(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();

            // Загружаем настройки приложения из файла
            var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
            settingsService.Load();

            // Инициализируем и применяем тему приложения на основе загруженных данных
            var themeService = scope.ServiceProvider.GetRequiredService<IThemeService>();
            themeService.Initialize();
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] App.LoadSettingsAndTheme failed: {ex.Message}");
#endif
        }
    }

    // TODO: Реализовать корректную остановку AppLauncher.Host при закрытии приложения (IHost.StopAsync).
}