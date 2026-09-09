using Abstractions.Core.DependencyInjection;
using Abstractions.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Repositories;

public class MasterDatabaseCreator : IMasterDatabaseCreator, IScopedService
{
    #region Fields

    private readonly string _masterConnectionString;

    #endregion Fields

    #region Constructors

    public MasterDatabaseCreator(IConfiguration configuration)
    {
        _masterConnectionString = configuration.GetConnectionString("MasterDatabase")
            ?? throw new InvalidOperationException("MasterDatabase bağlantı cümlesi bulunamadı.");
    }

    #endregion Constructors

    #region Methods

    public async Task CreateAsync(CancellationToken cancellationToken = default)
    {
        SqlConnectionStringBuilder connectionStringBuilder = new(_masterConnectionString);

        string databaseName = connectionStringBuilder.InitialCatalog;

        connectionStringBuilder.InitialCatalog = "master";

        await using SqlConnection connection = new(connectionStringBuilder.ConnectionString);

        await connection.OpenAsync(cancellationToken);

        string commandText = $"""
            IF DB_ID(N'{databaseName.Replace("'", "''")}') IS NULL
                CREATE DATABASE [{databaseName.Replace("]", "]]")}]
            """;

        await using SqlCommand command = new(commandText, connection);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    #endregion Methods
}