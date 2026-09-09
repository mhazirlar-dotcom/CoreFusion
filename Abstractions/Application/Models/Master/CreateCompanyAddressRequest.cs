namespace Abstractions.Application.Models.Master;

public record CreateCompanyAddressRequest
(
    Guid CityId ,
    Guid DistrictId ,
    Guid AddressTypeId ,
    string Address ,
    string PostalCode ,
    bool IsDefault
);