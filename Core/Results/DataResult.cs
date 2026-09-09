using Abstractions.Core.Results;

namespace Core.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    #region Properties

    public T Data { get; set; } = default!;

    #endregion Properties

    #region Constructors

    public DataResult()
    {

    }

    public DataResult(T data , bool success) : base(success)
    {
        Data = data;
    }

    public DataResult(T data , bool success , string message) : base(success , message)
    {
        Data = data;
    }

    #endregion Constructors
}