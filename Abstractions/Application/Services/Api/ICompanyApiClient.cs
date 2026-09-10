using Abstractions.Application.Models.Master;
using Abstractions.Core.Results;

namespace Abstractions.Application.Services.Api;

public interface ICompanyApiClient : IApiClient
{
    #region Methods

    Task<IDataResult<Guid>> CreateAsync(CreateCompanyRequest request , CancellationToken cancellationToken = default);
    Task<IDataResult<List<CompanyModel>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IDataResult<CompanyModel>> GetByIdAsync(Guid id , CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(UpdateCompanyRequest request , CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(Guid id , CancellationToken cancellationToken = default);

    #endregion Methods
}