using Abstractions.Application.Models.Master;
using FluentValidation;

namespace Application.Validation.Master;

public class CreateCompanyContactRequestValidator : AbstractValidator<CreateCompanyContactRequest>
{
    #region Constructors

    public CreateCompanyContactRequestValidator()
    {
        RuleFor(request => request.ContactTypeId)
            .NotEmpty();

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Title)
            .MaximumLength(150);

        RuleFor(request => request.Phone)
            .MaximumLength(30);

        RuleFor(request => request.MobilePhone)
            .MaximumLength(30);

        RuleFor(request => request.Email)
            .MaximumLength(200)
            .EmailAddress();
    }

    #endregion Constructors
}