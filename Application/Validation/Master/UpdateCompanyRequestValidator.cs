using Abstractions.Application.Models.Master;
using Abstractions.Core.DependencyInjection;
using FluentValidation;

namespace Application.Validation.Master;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>, ITransientService
{
    #region Constructors

    public UpdateCompanyRequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty();

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.ShortName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.TaxNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(request => request.TcIdentityNumber)
            .MaximumLength(11);

        RuleFor(request => request.TaxOfficeId)
            .NotEmpty();

        RuleFor(request => request.EstablishmentDate)
            .NotEmpty();

        RuleFor(request => request.ClosingDate)
            .GreaterThanOrEqualTo(request => request.EstablishmentDate)
            .When(request => request.ClosingDate.HasValue);

        RuleFor(request => request.TradeRegistryNumber)
            .MaximumLength(50);

        RuleFor(request => request.MersisNumber)
            .MaximumLength(20);

        RuleFor(request => request.Website)
            .MaximumLength(500);
    }

    #endregion Constructors
}