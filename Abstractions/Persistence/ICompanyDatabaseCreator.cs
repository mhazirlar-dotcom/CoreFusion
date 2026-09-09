namespace Abstractions.Persistence;

public interface ICompanyDatabaseCreator
{
    #region Methods

    Task CreateAsync(string databaseName , CancellationToken cancellationToken = default);

    #endregion Methods
}