using System.Text.Json;
using Domain.Master.Locations;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Master;

public static class CitySeedData
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
            "cities.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("City seed JSON dosyası bulunamadı." , filePath);
        }

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<CitySeedModel> seedCities = JsonSerializer.Deserialize<List<CitySeedModel>>(json, options)
            ?? [];

        List<City> existingCities = await context.Set<City>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        Dictionary<Guid, City> existingCityDictionary = existingCities
            .GroupBy(city => city.Id)
            .ToDictionary(group => group.Key, group => group.First());

        HashSet<Guid> processedIds = [];

        foreach (CitySeedModel seedCity in seedCities)
        {
            if (!processedIds.Add(seedCity.Id))
            {
                continue;
            }

            if (existingCityDictionary.TryGetValue(seedCity.Id , out City? existingCity))
            {
                City trackedCity = await context.Set<City>()
                    .FirstAsync(city => city.Id == existingCity.Id, cancellationToken);

                trackedCity.Name = seedCity.Name;
                trackedCity.Code = seedCity.Code;
                trackedCity.IsActive = seedCity.IsActive;

                continue;
            }

            City city = new()
            {
                Id = seedCity.Id,
                Code = seedCity.Code,
                Name = seedCity.Name,
                IsActive = seedCity.IsActive
            };

            context.Set<City>().Add(city);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion Methods

    #region Nested Types

    private sealed class CitySeedModel
    {
        #region Properties

        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        #endregion Properties
    }

    #endregion Nested Types
}