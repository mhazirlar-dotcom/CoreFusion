using Domain.Common;
using Domain.Master.Companies;

namespace Domain.Master.Locations;

public class City : Entity<Guid>
{
    #region Properties

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<District> Districts { get; set; } = [];

    public ICollection<CompanyAddress> CompanyAddresses { get; set; } = [];

    #endregion Properties
}