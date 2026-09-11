using Abstractions.Core.DependencyInjection;
using Abstractions.Persistence;

namespace Infrastructure.Persistence.Context;

public class CurrentCompanyContext : ICurrentCompanyContext, IScopedService
{
    #region Properties

    public Guid CompanyId { get; private set; }

    public string DatabaseName { get; private set; } = string.Empty;

    #endregion Properties

    #region Methods

    public void SetCompany(Guid companyId , string databaseName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        if (companyId == Guid.Empty)
        {
            throw new ArgumentException("CompanyId boş olamaz." , nameof(companyId));
        }

        CompanyId = companyId;
        DatabaseName = databaseName;
    }

    #endregion Methods
}