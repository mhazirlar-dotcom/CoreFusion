namespace Abstractions.Application.Models.Master;

public record CompanyContextResponse(Guid CompanyId , string ExpectedDatabaseName , string ConnectedDatabaseName , bool CanConnect);