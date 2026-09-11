using Abstractions.Core.DependencyInjection;
using Abstractions.Domain;
using Abstractions.Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories;

public class CompanyRepository<TEntity, TKey>(CoreFusionDbContext context) : ICompanyRepository<TEntity , TKey>, IScopedService where TEntity : class, IEntity<TKey> where TKey : notnull
{
    #region Fields

    private readonly CoreFusionDbContext _context = context;

    #endregion Fields

    #region Methods

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity , bool>>? filter = null , CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity , bool>> filter , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        TEntity? entity = await _context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(filter, cancellationToken);

        return entity ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} bulunamadı.");
    }

    public async Task AddAsync(TEntity entity , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Set<TEntity>().AddAsync(entity , cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion Methods
}