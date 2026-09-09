namespace Abstractions.Core.Validation;

public interface IValidationService
{
    #region Methods

    Task ValidateAsync<T>(T instance , CancellationToken cancellationToken = default);

    #endregion Methods
}