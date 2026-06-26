#nullable enable
using Microsoft.Extensions.Hosting;

namespace PortoPattern.DI;

/// <summary>
/// Статический класс для сборки и конфигурации хоста приложения.
/// Объединяет все регистрации: UI, Навигацию, Стили и Core-логику.
/// </summary>
public static class AppComposition
{
    /// <summary>
    /// Конфигурирует Generic Host, добавляя все слои приложения.
    /// </summary>
    /// <param name="builder">Строитель хоста</param>
    /// <returns>Сконфигурированный строитель хоста</returns>
    public static IHostBuilder BuildAppHost(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // 1. Слой Core (Бизнес-логика, сканеры, фильтры)
            // Добавляем наш новый метод расширения
            services.AddCoreServices();

            // 2. Слой UI (ViewModels, Pages, Navigation)
            services.AddUiServices();


            // Здесь в будущем можно добавить:
            // services.AddLogging();
            // services.AddDatabaseServices(); (если будет SQLite)
        });

        return builder;
    }
}