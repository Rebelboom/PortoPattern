#nullable enable

// File: App.xaml.cs
// Description: Точка входа WinUI 3 приложения. Управляет жизненным циклом окна и инициализацией навигации через DI.
// Правки: Полностью вырезан старый слой рантайм-управления темами (ThemeManager), удален не существующий using.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using PortoPattern.DI;

namespace PortoPattern;

/// <summary>
/// Точка входа приложения.
/// Отвечает за:
/// - создание окна
/// - инициализацию DI
/// - запуск UI
/// </summary>
public partial class App : Application
{
    private Window? m_window;

    public App()
    {
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

    // TODO: Реализовать корректную остановку AppLauncher.Host при закрытии приложения (IHost.StopAsync).
}