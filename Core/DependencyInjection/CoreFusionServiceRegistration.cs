using Abstractions.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.DependencyInjection;

public static class CoreFusionServiceRegistration
{
    #region Methods

    public static IServiceCollection AddCoreFusionServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return services;
    }
    public static IServiceCollection AddCoreFusionAssembly(this IServiceCollection services , Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
            .AsSelfWithInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
            .AsSelfWithInterfaces()
            .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
            .AsSelfWithInterfaces()
            .WithSingletonLifetime());

        return services;
    }

    #endregion Methods
}