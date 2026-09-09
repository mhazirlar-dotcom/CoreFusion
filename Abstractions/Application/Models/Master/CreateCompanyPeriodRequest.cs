namespace Abstractions.Application.Models.Master;

public record CreateCompanyPeriodRequest(
    Guid CompanyId ,
    DateOnly StartDate ,
    DateOnly EndDate
);