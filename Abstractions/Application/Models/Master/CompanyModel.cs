namespace Abstractions.Application.Models.Master;

public record CompanyModel(
    Guid Id ,
    string Name ,
    string ShortName ,
    string TaxNumber ,
    string TcIdentityNumber ,
    Guid TaxOfficeId ,
    string TaxOfficeName ,
    DateOnly EstablishmentDate ,
    DateOnly? ClosingDate ,
    string TradeRegistryNumber ,
    string MersisNumber ,
    string Website ,
    string DatabaseName ,
    bool IsActive
);