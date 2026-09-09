namespace Abstractions.Core.Results;

public interface IDataResult<T> : IResult
{
    #region Properties

    T Data { get; set; }

    #endregion Properties
}