using Abstractions.Core.DependencyInjection;
using Abstractions.Persistence;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Seed.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ApplicationDatabaseInitializer : IApplicationDatabaseInitializer, IScopedService
{
    #region Fields

    private readonly CoreFusionMasterDbContext _context;
    private readonly IMasterDatabaseCreator _databaseCreator;

    #endregion Fields

    #region Constructors

    public ApplicationDatabaseInitializer(CoreFusionMasterDbContext context , IMasterDatabaseCreator databaseCreator)
    {
        _context = context;
        _databaseCreator = databaseCreator;
    }

    #endregion Constructors

    #region Methods

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _databaseCreator.CreateAsync(cancellationToken);

        await _context.Database.MigrateAsync(cancellationToken);

        await MasterDataSeeder.SeedAsync(_context , cancellationToken);
    }

    #endregion Methods
}