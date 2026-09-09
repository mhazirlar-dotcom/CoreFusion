namespace Abstractions.Application.Models.Master;

public record CompanyPeriodModel(
    Guid Id ,
    Guid CompanyId ,
    DateOnly StartDate ,
    DateOnly EndDate ,
    bool IsActive
);