namespace Abstractions.Core.Exceptions;

public interface IApplicationException
{
    #region Properties

    string Code { get; }

    #endregion Properties
}