using Abstractions.Core.Exceptions;

namespace Core.Exceptions;

public class CoreApplicationException : Exception, IApplicationException
{
    #region Properties

    public string Code { get; }

    #endregion Properties

    #region Constructors

    public CoreApplicationException(string code , string message) : base(message)
    {
        Code = code;
    }

    public CoreApplicationException(string code , string message , Exception innerException) : base(message , innerException)
    {
        Code = code;
    }

    #endregion Constructors
}