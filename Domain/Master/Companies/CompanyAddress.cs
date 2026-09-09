using Domain.Common;
using Domain.Master.Definitions;
using Domain.Master.Locations;

namespace Domain.Master.Companies;

public class CompanyAddress : Entity<Guid>
{
    #region Properties

    public Guid CompanyId { get; set; }

    public Guid CityId { get; set; }

    public Guid DistrictId { get; set; }

    public Guid AddressTypeId { get; set; }

    public string Address { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public Company Company { get; set; } = null!;

    public City City { get; set; } = null!;

    public District District { get; set; } = null!;

    public AddressType AddressType { get; set; } = null!;

    #endregion Properties
}