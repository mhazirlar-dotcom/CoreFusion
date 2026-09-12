namespace Abstractions.Application.Models.Master;

public record UpdateCompanyRequest(
    Guid Id ,
    string Name ,
    string ShortName ,
    string TaxNumber ,
    string TcIdentityNumber ,
    Guid TaxOfficeId ,
    DateOnly EstablishmentDate ,
    DateOnly? ClosingDate ,
    string TradeRegistryNumber ,
    string MersisNumber ,
    string Website
);