using Abstractions.Core.DependencyInjection;
using Abstractions.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Repositories;

public class CompanyDatabaseCreator : ICompanyDatabaseCreator, IScopedService
{
    #region Fields

    private readonly string _masterConnectionString;

    #endregion Fields

    #region Constructors

    public CompanyDatabaseCreator(IConfiguration configuration)
    {
        _masterConnectionString = configuration.GetConnectionString("MasterDatabase")
            ?? throw new InvalidOperationException("MasterDatabase bağlantı cümlesi bulunamadı.");
    }

    #endregion Constructors

    #region Methods

    public async Task CreateAsync(string databaseName , CancellationToken cancellationToken = default)
    {
        SqlConnectionStringBuilder connectionStringBuilder = new(_masterConnectionString);

        await using SqlConnection connection = new(connectionStringBuilder.ConnectionString);

        await connection.OpenAsync(cancellationToken);

        string escapedDatabaseName = databaseName.Replace("]", "]]");
        string escapedDatabaseNameForSql = databaseName.Replace("'", "''");

        string commandText = $"""
            IF DB_ID(N'{escapedDatabaseNameForSql}') IS NULL
                CREATE DATABASE [{escapedDatabaseName}]
            """;

        await using SqlCommand command = new(commandText, connection);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    #endregion Methods
}