namespace Abstractions.Application.Models.Master;

public record CreateCompanyContactRequest
(
    Guid ContactTypeId ,
    string FirstName ,
    string LastName ,
    string Title ,
    string Phone ,
    string MobilePhone ,
    string Email ,
    bool IsAuthorized ,
    bool IsDefault
);