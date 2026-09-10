using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
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

    public static IServiceCollection AddCoreFusionAssembly(this IServiceCollection services , Assembly assembly , string? apiBaseUrl = null)
    {
        ArgumentNullException.ThrowIfNull(assembly);

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

        if (!string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            RegisterApiClients(services , assembly , apiBaseUrl);
        }

        return services;
    }

    private static void RegisterApiClients(IServiceCollection services , Assembly assembly , string apiBaseUrl)
    {
        Uri baseAddress = new(apiBaseUrl);

        services.AddHttpClient();

        services.Configure<HttpClientFactoryOptions>(Options.DefaultName , options =>
        {
            options.HttpClientActions.Add(client =>
            {
                client.BaseAddress = baseAddress;
            });
        });

        IEnumerable<Type> implementationTypes = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => typeof(IApiClient).IsAssignableFrom(type));

        foreach (Type implementationType in implementationTypes)
        {
            IEnumerable<Type> serviceTypes = implementationType
                .GetInterfaces()
                .Where(interfaceType => interfaceType != typeof(IApiClient))
                .Where(interfaceType => typeof(IApiClient).IsAssignableFrom(interfaceType));

            foreach (Type serviceType in serviceTypes)
            {
                services.AddTransient(serviceType , implementationType);
            }
        }
    }

    #endregion Methods
}