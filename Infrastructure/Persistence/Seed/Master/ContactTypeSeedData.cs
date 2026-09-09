using System.Text.Json;
using Domain.Master.Definitions;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed.Master;

public static class ContactTypeSeedData
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
            "contacttype.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("ContactType seed JSON dosyası bulunamadı." , filePath);
        }

        string json = await File.ReadAllTextAsync(filePath, cancellationToken);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<ContactTypeSeedModel> seedContactTypes = JsonSerializer.Deserialize<List<ContactTypeSeedModel>>(json, options)
            ?? [];

        List<ContactType> existingContactTypes = await context.Set<ContactType>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        HashSet<Guid> existingContactTypeIds = existingContactTypes
            .Select(contactType => contactType.Id)
            .ToHashSet();

        HashSet<Guid> processedIds = [];

        foreach (ContactTypeSeedModel seedContactType in seedContactTypes)
        {
            if (!processedIds.Add(seedContactType.Id))
            {
                continue;
            }

            if (existingContactTypeIds.Contains(seedContactType.Id))
            {
                ContactType? existingContactType = await context.Set<ContactType>()
                    .FirstOrDefaultAsync(contactType => contactType.Id == seedContactType.Id, cancellationToken);

                if (existingContactType is not null)
                {
                    existingContactType.Name = seedContactType.Name;
                    existingContactType.Code = seedContactType.Code;
                    existingContactType.IsActive = seedContactType.IsActive;
                }

                continue;
            }

            ContactType contactType = new()
            {
                Id = seedContactType.Id,
                Name = seedContactType.Name,
                Code = seedContactType.Code,
                IsActive = seedContactType.IsActive
            };

            context.Set<ContactType>().Add(contactType);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion Methods

    #region Nested Types

    private sealed class ContactTypeSeedModel
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