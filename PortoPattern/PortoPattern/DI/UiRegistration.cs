// ****************************************************************************
// Файл: UiRegistration.cs
// Описание: Централизованная регистрация UI-сервисов, ViewModels и страниц.
// ****************************************************************************

#nullable enable

using System;
using Microsoft.Extensions.DependencyInjection;
using PortoPattern.Interfaces;
using PortoPattern.Services;
using PortoPattern.Navigation;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Navigation.Services;
using PortoPattern.ViewModels;
using PortoPattern.Views;

namespace PortoPattern.DI;

public static class UiRegistration
{
    public static IServiceCollection AddUiServices(this IServiceCollection services)
    {
        // =========================================================
        // БЛОК 1: ИНФРАСТРУКТУРА НАВИГАЦИИ
        // =========================================================
        services.AddSingleton<ViewFactory>();
        services.AddSingleton<IViewFactory>(sp => sp.GetRequiredService<ViewFactory>());
        services.AddSingleton<IViewRegistry>(sp => sp.GetRequiredService<ViewFactory>());
        services.AddSingleton<INavigationService, NavigationService>();

        // =========================================================
        // БЛОК 2: СИСТЕМНЫЕ СЕРВИСЫ
        // =========================================================
        services.AddSingleton<IWindowProvider, WindowProvider>();
        services.AddSingleton<IFilePickerService, FilePickerService>();

        // =========================================================
        // БЛОК 3: VIEWMODELS (Основные страницы)
        // Примечание: FolderCardViewModel НЕ регистрируется, так как 
        // создается динамически в DetailsViewModel.
        // =========================================================
        services.AddSingleton<ShellViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ResultViewModel>();
        services.AddTransient<DetailsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<BlackListViewModel>();

        // =========================================================
        // БЛОК 4: PAGES (Представления)
        // =========================================================
        services.AddTransient<ShellPage>();
        services.AddTransient<HomePage>();
        services.AddTransient<DashboardPage>();
        services.AddTransient<ResultPage>();
        services.AddTransient<DetailsPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<BlacklistPage>();

        services.AddSingleton<MainWindow>();

        return services;
    }

    public static void MapNavigation(this IServiceProvider provider)
    {
        var registry = provider.GetRequiredService<IViewRegistry>();

        registry.Register<ShellViewModel, ShellPage>();
        registry.Register<HomeViewModel, HomePage>();
        registry.Register<DashboardViewModel, DashboardPage>();
        registry.Register<ResultViewModel, ResultPage>();
        registry.Register<DetailsViewModel, DetailsPage>();
        registry.Register<SettingsViewModel, SettingsPage>();
        registry.Register<BlackListViewModel, BlacklistPage>();
    }
}