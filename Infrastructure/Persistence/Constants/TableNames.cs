using Domain.Master.Definitions;

namespace Infrastructure.Persistence.Constants;

public static class TableNames
{
    #region Master
    public const string Companies = nameof(Companies);
    public const string CompanyAddresses = nameof(CompanyAddresses);
    public const string CompanyContacts = nameof(CompanyContacts);
    public const string Cities = nameof(Cities);
    public const string Districts = nameof(Districts);
    public const string TaxOffices = nameof(TaxOffices);
    public const string AddressTypes = nameof(AddressTypes);
    public const string ContactTypes = nameof(ContactTypes);

    #endregion Master

    #region Company
    public const string Categories = nameof(Categories);
    public const string Products = nameof(Products);

    #endregion Company
}
