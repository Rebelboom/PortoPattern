#nullable enable
// File: App.xaml.cs

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using PortoPattern.DI;

namespace PortoPattern;

public partial class App : Application
{
    private Window? m_window;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            var provider = AppLauncher.Host?.Services;

            if (provider == null)
            {
                throw new InvalidOperationException("Host не инициализирован. Проверьте последовательность запуска в AppLauncher.");
            }

            provider.MapNavigation();

            m_window = provider.GetRequiredService<MainWindow>();

            m_window.Activate();
        }
        catch (Exception)
        {
            throw;
        }
    }
}