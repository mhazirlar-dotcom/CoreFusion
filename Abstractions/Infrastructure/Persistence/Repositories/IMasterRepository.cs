using Abstractions.Domain;
using System.Linq.Expressions;

namespace Abstractions.Infrastructure.Persistence.Repositories;

public interface IMasterRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
    #region Methods

    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity , bool>>? filter = null , CancellationToken cancellationToken = default);

    Task<TEntity> GetAsync(Expression<Func<TEntity , bool>> filter , CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity , CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity , CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity , CancellationToken cancellationToken = default);

    #endregion Methods
}