using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Context;

public class CoreFusionMasterDbContextFactory : IDesignTimeDbContextFactory<CoreFusionMasterDbContext>
{
    #region Methods

    public CoreFusionMasterDbContext CreateDbContext(string[] args)
    {
        string wpfPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Wpf");

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(wpfPath, "appsettings.json"), optional: false)
            .Build();

        string connectionString = configuration.GetConnectionString("MasterDatabase")
            ?? throw new InvalidOperationException("MasterDatabase bağlantı cümlesi bulunamadı.");

        DbContextOptionsBuilder<CoreFusionMasterDbContext> optionsBuilder = new();

        optionsBuilder.UseSqlServer(connectionString);

        return new CoreFusionMasterDbContext(optionsBuilder.Options);
    }

    #endregion Methods
}