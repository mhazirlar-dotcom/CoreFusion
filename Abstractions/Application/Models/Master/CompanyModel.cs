namespace Abstractions.Application.Models.Master;

public record CompanyModel(
    Guid Id ,
    string Name ,
    string ShortName ,
    string TaxNumber ,
    Guid TaxOfficeId ,
    string TradeRegistryNumber ,
    string MersisNumber ,
    string Website ,
    string DatabaseName ,
    bool IsActive
);