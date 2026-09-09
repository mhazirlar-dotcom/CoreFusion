using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Validation;
using Core.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Validation;

public class ValidationService(IEnumerable<IValidator> validators) : IValidationService, IScopedService
{
    #region Fields

    private readonly IEnumerable<IValidator> _validators = validators;

    #endregion Fields

    #region Methods

    public async Task ValidateAsync<T>(T instance , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);

        IValidator[] matchingValidators = [.. _validators.Where(validator => validator.CanValidateInstancesOfType(typeof(T)))];

        if (matchingValidators.Length == 0)
        {
            throw new InvalidOperationException($"{typeof(T).Name} için validator bulunamadı.");
        }

        List<ValidationFailure> failures = [];

        foreach (IValidator validator in matchingValidators)
        {
            ValidationContext<T> context = new(instance);
            ValidationResult result = await validator.ValidateAsync(context, cancellationToken);

            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
        {
            return;
        }

        string message = string.Join(Environment.NewLine, failures.Select(failure => failure.ErrorMessage));

        throw new CoreValidationException(message);
    }

    #endregion Methods
}