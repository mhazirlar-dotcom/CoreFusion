using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Context;

public class CoreFusionDbContextFactory : IDesignTimeDbContextFactory<CoreFusionDbContext>
{
    #region Methods

    public CoreFusionDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CoreFusionDbContext> optionsBuilder = new();

        optionsBuilder.UseSqlServer("Server=.; Database=CoreFusionCompanyMigration; Trusted_Connection=True; TrustServerCertificate=True;");

        return new CoreFusionDbContext(optionsBuilder.Options);
    }

    #endregion Methods
}