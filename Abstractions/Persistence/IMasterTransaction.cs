namespace Abstractions.Persistence;

public interface IMasterTransaction
{
    #region Methods

    Task ExecuteAsync(Func<CancellationToken , Task> action , CancellationToken cancellationToken = default);

    #endregion Methods
}