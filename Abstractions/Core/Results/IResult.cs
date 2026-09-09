namespace Abstractions.Core.Results;

public interface IResult
{
    #region Properties

    bool Success { get; set; }

    string Message { get; set; }

    #endregion Properties
}