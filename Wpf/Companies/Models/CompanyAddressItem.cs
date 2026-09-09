namespace Wpf.Companies.Models;

public class CompanyAddressItem
{
    #region Properties

    public Guid Id { get; set; }
    public Guid AddressTypeId { get; set; }
    public Guid CityId { get; set; }
    public Guid DistrictId { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    #endregion Properties
}