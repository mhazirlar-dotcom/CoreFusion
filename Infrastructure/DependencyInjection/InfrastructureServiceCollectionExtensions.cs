using Core.DependencyInjection;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    #region Methods

    public static IServiceCollection AddInfrastructure(this IServiceCollection services , IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("MasterDatabase")
            ?? throw new InvalidOperationException("MasterDatabase bağlantı cümlesi bulunamadı.");

        services.AddDbContext<CoreFusionMasterDbContext>(options =>
        {
            options.UseSqlServer(connectionString , sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(CoreFusionMasterDbContext).Assembly.FullName);
            });
        });

        services.AddCoreFusionAssembly(typeof(InfrastructureServiceCollectionExtensions).Assembly);

        return services;
    }

    #endregion Methods
}