using Domain.Common;
using Domain.Master.Companies;

namespace Domain.Master.Definitions;

public class AddressType : Entity<Guid>
{
    #region Properties

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<CompanyAddress> CompanyAddresses { get; set; } = [];

    #endregion Properties
}