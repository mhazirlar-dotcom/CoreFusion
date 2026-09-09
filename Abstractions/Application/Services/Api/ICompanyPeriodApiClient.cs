using Abstractions.Application.Models.Master;
using Abstractions.Core.Results;

namespace Abstractions.Application.Services.Api;

public interface ICompanyPeriodApiClient
{
    #region Methods

    Task<IDataResult<Guid>> CreateAsync(CreateCompanyPeriodRequest request , CancellationToken cancellationToken = default);

    Task<IDataResult<List<CompanyPeriodModel>>> GetByCompanyIdAsync(Guid companyId , CancellationToken cancellationToken = default);

    Task<IDataResult<CompanyPeriodModel>> GetByIdAsync(Guid companyId , Guid id , CancellationToken cancellationToken = default);

    Task<IResult> UpdateAsync(Guid companyId , UpdateCompanyPeriodRequest request , CancellationToken cancellationToken = default);

    Task<IResult> DeleteAsync(Guid companyId , Guid id , CancellationToken cancellationToken = default);

    #endregion Methods
}