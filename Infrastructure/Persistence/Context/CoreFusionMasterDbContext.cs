using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class CoreFusionMasterDbContext(DbContextOptions<CoreFusionMasterDbContext> options) : DbContext(options)
{
    #region Methods

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreFusionMasterDbContext).Assembly , type => type.Namespace?.StartsWith("Infrastructure.Persistence.Configurations.Master") == true);
    }

    #endregion Methods
}