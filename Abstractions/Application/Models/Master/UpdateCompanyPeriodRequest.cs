namespace Abstractions.Application.Models.Master;

public record UpdateCompanyPeriodRequest(
    Guid Id ,
    DateOnly StartDate ,
    DateOnly EndDate ,
    bool IsActive
);