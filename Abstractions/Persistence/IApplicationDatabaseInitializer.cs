namespace Abstractions.Persistence;

public interface IApplicationDatabaseInitializer
{
    #region Methods

    Task InitializeAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}