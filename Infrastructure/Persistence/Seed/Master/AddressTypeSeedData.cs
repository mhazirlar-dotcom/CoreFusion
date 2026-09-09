using System.Text.Json;
using Domain.Master.Definitions;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Master;

public static class AddressTypeSeedData
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
            "addresstype.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("AddressType seed JSON dosyası bulunamadı." , filePath);
        }

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<AddressTypeSeedModel> seedAddressTypes = JsonSerializer.Deserialize<List<AddressTypeSeedModel>>(json, options)
            ?? [];

        List<AddressType> existingAddressTypes = await context.Set<AddressType>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        HashSet<Guid> existingAddressTypeIds = existingAddressTypes
            .Select(addressType => addressType.Id)
            .ToHashSet();

        HashSet<string> existingAddressTypeCodes = existingAddressTypes
            .Select(addressType => addressType.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        HashSet<Guid> processedIds = [];

        foreach (AddressTypeSeedModel seedAddressType in seedAddressTypes)
        {
            if (!processedIds.Add(seedAddressType.Id))
            {
                continue;
            }

            if (existingAddressTypeIds.Contains(seedAddressType.Id))
            {
                AddressType? existingAddressType = await context.Set<AddressType>()
                    .FirstOrDefaultAsync(addressType => addressType.Id == seedAddressType.Id, cancellationToken);

                if (existingAddressType is not null)
                {
                    existingAddressType.Name = seedAddressType.Name;
                    existingAddressType.Code = seedAddressType.Code;
                    existingAddressType.IsActive = seedAddressType.IsActive;
                }

                continue;
            }

            AddressType addressType = new()
            {
                Id = seedAddressType.Id,
                Name = seedAddressType.Name,
                Code = seedAddressType.Code,
                IsActive = seedAddressType.IsActive
            };

            context.Set<AddressType>().Add(addressType);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion Methods

    #region Nested Types

    private sealed class AddressTypeSeedModel
    {
        #region Properties

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        #endregion Properties
    }

    #endregion Nested Types
}
