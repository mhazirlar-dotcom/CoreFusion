using Abstractions.Core.DependencyInjection;
using Abstractions.Persistence;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Repositories;

public class MasterTransaction(CoreFusionMasterDbContext context) : IMasterTransaction, IScopedService
{
    #region Fields

    private readonly CoreFusionMasterDbContext _context = context;

    #endregion Fields

    #region Methods

    public async Task ExecuteAsync(Func<CancellationToken , Task> action , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    #endregion Methods
}