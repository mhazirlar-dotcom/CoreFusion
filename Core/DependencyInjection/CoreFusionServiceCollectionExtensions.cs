using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyInjection;

public static class CoreFusionServiceCollectionExtensions
{
    #region Methods

    public static IServiceCollection AddCoreFusion(this IServiceCollection services)
    {
        services.AddCoreFusionServices();

        return services;
    }

    #endregion Methods
}