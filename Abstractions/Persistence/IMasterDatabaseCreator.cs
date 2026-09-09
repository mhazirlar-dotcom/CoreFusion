namespace Abstractions.Persistence;

public interface IMasterDatabaseCreator
{
    #region Methods

    Task CreateAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}