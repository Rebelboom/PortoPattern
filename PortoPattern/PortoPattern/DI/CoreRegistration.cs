#nullable enable
using Microsoft.Extensions.DependencyInjection;
using PortoPattern.Core.Interfaces;
using PortoPattern.Core.Services;
using PortoPattern.Core.IgnorSpace;

namespace PortoPattern.DI;

public static class CoreRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileDiscoveryService, FileDiscoveryService>();

        services.AddSingleton<IgnorManager>();
        services.AddSingleton<IgnorRuleGenerator>();
        services.AddSingleton<IgnorFilterService>();

        return services;
    }
}