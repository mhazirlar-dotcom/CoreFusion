namespace Abstractions.Domain;

public interface IEntity<TKey> where TKey : notnull
{
    #region Properties

    TKey Id { get; set; }

    #endregion Properties
}