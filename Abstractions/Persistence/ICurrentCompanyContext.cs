namespace Abstractions.Persistence;

public interface ICurrentCompanyContext
{
    #region Properties

    Guid CompanyId { get; }

    string DatabaseName { get; }

    #endregion Properties

    #region Methods

    void SetCompany(Guid companyId , string databaseName);

    #endregion Methods
}