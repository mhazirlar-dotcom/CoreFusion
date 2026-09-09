using Abstractions.Core.Results;

namespace Core.Results;

public class Result : IResult
{
    #region Properties

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    #endregion Properties

    #region Constructors

    public Result()
    {

    }

    public Result(bool success)
    {
        Success = success;
    }

    public Result(bool success , string message) : this(success)
    {
        Message = message;
    }

    #endregion Constructors
}