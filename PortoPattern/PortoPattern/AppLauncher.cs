#nullable enable
// File: AppLauncher.cs

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using PortoPattern.DI;
using PortoPattern.Core.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortoPattern;

public static class AppLauncher
{
    public static IHost? Host { get; private set; }

    [STAThread]
    static void Main(string[] args)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();

        try
        {
            var keyInstance = AppInstance.FindOrRegisterForKey("PortoPatternSingleInstance");
            if (!keyInstance.IsCurrent)
            {
                var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                keyInstance.RedirectActivationToAsync(activationArgs).AsTask().GetAwaiter().GetResult();
                return;
            }

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .BuildAppHost()
                .Build();

            Host.Start();

            var settingsService = Host.Services.GetRequiredService<ISettingsService>();
            settingsService.Load();

            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);

                _ = new App();
            });
        }
        catch (Exception)
        {
            throw;
        }
    }
}