using System.Text.Json;
using Domain.Master.Tax;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Master;

public static class TaxOfficeSeedData
{
    #region Methods

    public static async Task SeedAsync(CoreFusionMasterDbContext context , CancellationToken cancellationToken = default)
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "Seed",
            "Master",
            "Data",
            "tax-offices.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("TaxOffice seed JSON dosyası bulunamadı." , filePath);
        }

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<TaxOfficeSeedModel> seedTaxOffices = JsonSerializer.Deserialize<List<TaxOfficeSeedModel>>(json, options)
            ?? [];

        List<TaxOffice> existingTaxOffices = await context.Set<TaxOffice>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        HashSet<Guid> existingTaxOfficeIds = existingTaxOffices
            .Select(taxOffice => taxOffice.Id)
            .ToHashSet();

        HashSet<Guid> processedIds = [];

        foreach (TaxOfficeSeedModel seedTaxOffice in seedTaxOffices)
        {
            if (!processedIds.Add(seedTaxOffice.Id))
            {
                continue;
            }

            if (existingTaxOfficeIds.Contains(seedTaxOffice.Id))
            {
                TaxOffice? existingTaxOffice = await context.Set<TaxOffice>()
                    .FirstOrDefaultAsync(taxOffice => taxOffice.Id == seedTaxOffice.Id, cancellationToken);

                if (existingTaxOffice is not null)
                {
                    existingTaxOffice.Name = seedTaxOffice.Name;
                    existingTaxOffice.Code = seedTaxOffice.Code;
                    existingTaxOffice.CityId = seedTaxOffice.CityId;
                    existingTaxOffice.IsActive = seedTaxOffice.IsActive;
                }

                continue;
            }

            TaxOffice taxOffice = new()
            {
                Id = seedTaxOffice.Id,
                Name = seedTaxOffice.Name,
                Code = seedTaxOffice.Code,
                CityId = seedTaxOffice.CityId,
                IsActive = seedTaxOffice.IsActive
            };

            context.Set<TaxOffice>().Add(taxOffice);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion Methods

    #region Nested Types

    private sealed class TaxOfficeSeedModel
    {
        #region Properties

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public Guid CityId { get; set; }
        public bool IsActive { get; set; }

        #endregion Properties
    }

    #endregion Nested Types
}