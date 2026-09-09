using Domain.Common;
using Domain.Master.Tax;

namespace Domain.Master.Companies;

public class Company : Entity<Guid>
{
    #region Properties

    public string Name { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;

    public string TaxNumber { get; set; } = string.Empty;

    public Guid TaxOfficeId { get; set; }

    public string TradeRegistryNumber { get; set; } = string.Empty;

    public string MersisNumber { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public string LogoPath { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public TaxOffice TaxOffice { get; set; } = null!;

    public ICollection<CompanyAddress> CompanyAddresses { get; set; } = [];

    public ICollection<CompanyContact> CompanyContacts { get; set; } = [];

    public ICollection<CompanyPeriod> CompanyPeriods { get; set; } = [];

    #endregion Properties
}