namespace Abstractions.Application.Models.Master;

public record CreateCompanyRequest
(
    string Name ,
    string ShortName ,
    string TaxNumber ,
    Guid TaxOfficeId ,
    string TradeRegistryNumber ,
    string MersisNumber ,
    string Website ,
    List<CreateCompanyAddressRequest> Addresses ,
    List<CreateCompanyContactRequest> Contacts
);