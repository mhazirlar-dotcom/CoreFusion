namespace Abstractions.Application.Models.Master;

public record UpdateCompanyRequest(
    Guid Id ,
    string Name ,
    string ShortName ,
    string TaxNumber ,
    Guid TaxOfficeId ,
    string TradeRegistryNumber ,
    string MersisNumber ,
    string Website
);