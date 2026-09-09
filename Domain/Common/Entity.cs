using Abstractions.Domain;

namespace Domain.Common;

public abstract class Entity<TKey> : IEntity<TKey> where TKey : notnull
{
    #region Properties

    public TKey Id { get; set; } = default!;

    #endregion Properties
}