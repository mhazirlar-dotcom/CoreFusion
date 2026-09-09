using Abstractions.Application.Models.Master;
using FluentValidation;

namespace Application.Validation.Master;

public class CreateCompanyAddressRequestValidator : AbstractValidator<CreateCompanyAddressRequest>
{
    #region Constructors

    public CreateCompanyAddressRequestValidator()
    {
        RuleFor(request => request.CityId)
            .NotEmpty();

        RuleFor(request => request.DistrictId)
            .NotEmpty();

        RuleFor(request => request.AddressTypeId)
            .NotEmpty();

        RuleFor(request => request.Address)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(request => request.PostalCode)
            .MaximumLength(20);
    }

    #endregion Constructors
}