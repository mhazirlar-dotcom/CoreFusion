namespace Abstractions.Persistence;

public interface ICompanyDatabaseMigrator
{
    #region Methods

    Task MigrateAsync(string databaseName , CancellationToken cancellationToken = default);

    #endregion Methods
}