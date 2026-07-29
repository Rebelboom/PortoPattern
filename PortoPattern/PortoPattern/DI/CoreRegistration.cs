#nullable enable
using Microsoft.Extensions.DependencyInjection;
using PortoPattern.Core.History;
using PortoPattern.Core.IgnorSpace;
using PortoPattern.Core.Interfaces;
using PortoPattern.Core.Services;
using PortoPattern.Core.Settings;

namespace PortoPattern.DI;

public static class CoreRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileDiscoveryService, FileDiscoveryService>();

        services.AddSingleton<IgnorManager>();
        services.AddSingleton<IgnorRuleGenerator>();
        services.AddSingleton<IgnorFilterService>();
        services.AddSingleton<IScanHistoryService, ScanHistoryService>();
        services.AddSingleton<ISettingsService, SettingsService>();

        return services;
    }
}