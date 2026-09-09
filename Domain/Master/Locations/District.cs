using Domain.Common;
using Domain.Master.Companies;

namespace Domain.Master.Locations;

public class District : Entity<Guid>
{
    #region Properties

    public Guid CityId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public City City { get; set; } = null!;

    public ICollection<CompanyAddress> CompanyAddresses { get; set; } = [];

    #endregion Properties
}