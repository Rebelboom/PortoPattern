#nullable enable

// File: AppLauncher.cs
// Description: Главный загрузчик системы. Обеспечивает инициализацию Generic Host, 
// обработку Single Instance (через AppLifecycle) и запуск UI-потока WinUI 3.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using PortoPattern.DI;

namespace PortoPattern;

/// <summary>
/// Главный загрузчик системы. Обеспечивает запуск Generic Host, 
/// валидацию DI-контейнера и старт графической подсистемы WinUI 3.
/// </summary>
public static class AppLauncher
{
    // Глобальная ссылка на хост для доступа к сервисам в App.xaml.cs[cite: 1]
    public static IHost? Host { get; private set; }

    /// <summary>
    /// Точка входа в приложение.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // Инициализация поддержки WinRT для C#. Необходима для корректного маршалинга объектов.[cite: 1]
        WinRT.ComWrappersSupport.InitializeComWrappers();

        try
        {
            // Single Instance Protection: регистрация ключа экземпляра приложения.[cite: 1]
            var keyInstance = AppInstance.FindOrRegisterForKey("PortoPatternSingleInstance");
            if (!keyInstance.IsCurrent)
            {
                // Перенаправление аргументов активации в существующий экземпляр[cite: 1]
                var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                keyInstance.RedirectActivationToAsync(activationArgs).AsTask().GetAwaiter().GetResult();
                return;
            }

            // Инициализация Host-контейнера через AppComposition (Porto Pattern DI).[cite: 1]
            // Используется CreateDefaultBuilder для базовой конфигурации логирования и настроек.
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .BuildAppHost()
                .Build();

            // Асинхронный запуск хоста. Позволяет службам IHostedService начать работу в фоне.[cite: 1]
            Host.Start();

            // Запуск UI-потока WinUI 3.[cite: 1]
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                // Установка контекста синхронизации для корректной работы async/await в UI потоке.[cite: 1]
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);

                // Создание экземпляра приложения. Инстанцирование App инициирует XAML-ресурсы.[cite: 1]
                _ = new App();
            });
        }
        catch (Exception ex)
        {
#if DEBUG
            // Вывод критических ошибок инициализации в консоль отладки[cite: 1]
            Console.WriteLine($"[CRITICAL ERROR] AppLauncher: {ex.Message}");
            Console.WriteLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
#endif
            throw;
        }
    }

    // TODO: Рассмотреть возможность использования IHostLifetime для корректного завершения работы Generic Host при закрытии всех окон WinUI.[cite: 1]
    // TODO: Добавить проверку готовности Host в методе Application.Start, если сервисы требуются сразу в конструкторе App.[cite: 1]
}