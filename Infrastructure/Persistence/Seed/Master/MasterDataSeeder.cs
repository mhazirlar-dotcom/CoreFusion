using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Seed.Master;

public static class MasterDataSeeder
{
    #region Methods

    public static async Task SeedAsync(CoreFusionMasterDbContext context , CancellationToken cancellationToken = default)
    {
        await CitySeedData.SeedAsync(context , cancellationToken);
        await DistrictSeedData.SeedAsync(context , cancellationToken);
        await TaxOfficeSeedData.SeedAsync(context , cancellationToken);
        await AddressTypeSeedData.SeedAsync(context , cancellationToken);
        await ContactTypeSeedData.SeedAsync(context , cancellationToken);
    }

    #endregion Methods
}