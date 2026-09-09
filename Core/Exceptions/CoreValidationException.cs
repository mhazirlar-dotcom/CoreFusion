namespace Core.Exceptions;

public class CoreValidationException : CoreApplicationException
{
    #region Constructors

    public CoreValidationException(string message) : base("VALIDATION_ERROR" , message)
    {

    }

    #endregion Constructors
}