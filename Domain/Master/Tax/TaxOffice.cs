using Domain.Common;
using Domain.Master.Companies;
using Domain.Master.Locations;

namespace Domain.Master.Tax;

public class TaxOffice : Entity<Guid>
{
    #region Properties

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; }

    public Guid CityId { get; set; }

    public bool IsActive { get; set; } = true;

    public City City { get; set; } = null!;

    public ICollection<Company> Companies { get; set; } = [];

    #endregion Properties
}