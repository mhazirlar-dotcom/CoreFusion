using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class CoreFusionDbContext(DbContextOptions<CoreFusionDbContext> options) : DbContext(options)
{
    #region Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreFusionDbContext).Assembly , type => type.Namespace?.StartsWith("Infrastructure.Persistence.Configurations.Companies") == true);
    }

    #endregion Methods
}