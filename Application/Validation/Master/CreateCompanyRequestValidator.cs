using Abstractions.Application.Models.Master;
using Abstractions.Core.DependencyInjection;
using FluentValidation;

namespace Application.Validation.Master;

public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>, ITransientService
{
    #region Constructors

    public CreateCompanyRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.ShortName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.TaxNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(request => request.TaxOfficeId)
            .NotEmpty();

        RuleFor(request => request.TradeRegistryNumber)
            .MaximumLength(50);

        RuleFor(request => request.MersisNumber)
            .MaximumLength(20);

        RuleFor(request => request.Website)
            .MaximumLength(500);

        RuleForEach(request => request.Addresses)
            .SetValidator(new CreateCompanyAddressRequestValidator());

        RuleForEach(request => request.Contacts)
            .SetValidator(new CreateCompanyContactRequestValidator());

        RuleFor(request => request.Addresses)
            .Must(addresses => addresses.Count(address => address.IsDefault) <= 1)
            .WithMessage("Bir firma için yalnızca bir varsayılan adres olabilir.");

        RuleFor(request => request.Contacts)
            .Must(contacts => contacts.Count(contact => contact.IsDefault) <= 1)
            .WithMessage("Bir firma için yalnızca bir varsayılan iletişim olabilir.");
    }

    #endregion Constructors
}