using Abstractions.Persistence;
using Core.DependencyInjection;
using Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    #region Methods

    public static IServiceCollection AddInfrastructure(this IServiceCollection services , IConfiguration configuration)
    {
        string masterConnectionString = configuration.GetConnectionString("MasterDatabase")
            ?? throw new InvalidOperationException("MasterDatabase bağlantı cümlesi bulunamadı.");

        services.AddDbContext<CoreFusionMasterDbContext>(options =>
        {
            options.UseSqlServer(masterConnectionString , sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(CoreFusionMasterDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<CoreFusionDbContext>(serviceProvider =>
        {
            ICurrentCompanyContext currentCompanyContext = serviceProvider.GetRequiredService<ICurrentCompanyContext>();

            if (currentCompanyContext.CompanyId == Guid.Empty)
            {
                throw new InvalidOperationException("Aktif firma belirlenmeden şirket veritabanı kullanılamaz.");
            }

            if (string.IsNullOrWhiteSpace(currentCompanyContext.DatabaseName))
            {
                throw new InvalidOperationException("Aktif firmanın veritabanı adı bulunamadı.");
            }

            SqlConnectionStringBuilder connectionStringBuilder = new(masterConnectionString)
            {
                InitialCatalog = currentCompanyContext.DatabaseName
            };

            DbContextOptions<CoreFusionDbContext> options = new DbContextOptionsBuilder<CoreFusionDbContext>()
                .UseSqlServer(connectionStringBuilder.ConnectionString , sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(CoreFusionDbContext).Assembly.FullName);
                })
                .Options;

            return new CoreFusionDbContext(options);
        });

        services.AddCoreFusionAssembly(typeof(InfrastructureServiceCollectionExtensions).Assembly);

        return services;
    }

    #endregion Methods
}