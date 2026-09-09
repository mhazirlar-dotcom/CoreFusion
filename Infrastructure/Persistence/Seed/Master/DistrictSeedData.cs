using System.Text.Json;
using Domain.Master.Locations;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Master;

public static class DistrictSeedData
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
            "districts.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("District seed JSON dosyası bulunamadı." , filePath);
        }

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<DistrictSeedModel> seedDistricts = JsonSerializer.Deserialize<List<DistrictSeedModel>>(json, options)
    ?? [];

        List<District> existingDistricts = await context.Set<District>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        HashSet<Guid> existingDistrictIds = existingDistricts
            .Select(district => district.Id)
            .ToHashSet();

        HashSet<Guid> processedIds = [];

        foreach (DistrictSeedModel seedDistrict in seedDistricts)
        {
            if (!processedIds.Add(seedDistrict.Id))
            {
                continue;
            }

            if (existingDistrictIds.Contains(seedDistrict.Id))
            {
                District? existingDistrict = await context.Set<District>()
                    .FirstOrDefaultAsync(district => district.Id == seedDistrict.Id, cancellationToken);

                if (existingDistrict is not null)
                {
                    existingDistrict.CityId = seedDistrict.CityId;
                    existingDistrict.Name = seedDistrict.Name;
                    existingDistrict.IsActive = seedDistrict.IsActive;
                }

                continue;
            }

            District district = new()
            {
                Id = seedDistrict.Id,
                CityId = seedDistrict.CityId,
                Name = seedDistrict.Name,
                IsActive = seedDistrict.IsActive
            };

            context.Set<District>().Add(district);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion Methods

    #region Nested Types

    private sealed class DistrictSeedModel
    {
        #region Properties

        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        #endregion Properties
    }

    #endregion Nested Types
}