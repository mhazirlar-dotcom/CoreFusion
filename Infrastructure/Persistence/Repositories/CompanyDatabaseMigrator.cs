using Abstractions.Core.DependencyInjection;
using Abstractions.Persistence;
using Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Repositories;

public class CompanyDatabaseMigrator : ICompanyDatabaseMigrator, IScopedService
{
    #region Fields

    private readonly string _masterConnectionString;

    #endregion Fields

    #region Constructors

    public CompanyDatabaseMigrator(IConfiguration configuration)
    {
        _masterConnectionString = configuration.GetConnectionString("MasterDatabase")
            ?? throw new InvalidOperationException("MasterDatabase bağlantı cümlesi bulunamadı.");
    }

    #endregion Constructors

    #region Methods

    public async Task MigrateAsync(string databaseName , CancellationToken cancellationToken = default)
    {
        SqlConnectionStringBuilder connectionStringBuilder = new(_masterConnectionString)
        {
            InitialCatalog = databaseName
        };

        DbContextOptions<CoreFusionDbContext> options = new DbContextOptionsBuilder<CoreFusionDbContext>()
            .UseSqlServer(connectionStringBuilder.ConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(CoreFusionDbContext).Assembly.FullName);
            })
            .Options;

        await using CoreFusionDbContext context = new(options);

        await context.Database.MigrateAsync(cancellationToken);
    }

    #endregion Methods
}