using Abstractions.Application.Models.Master;

namespace Abstractions.Application.Services.Api;

public interface ICompanyContextApiClient : IApiClient
{
    #region Methods

    Task<CompanyContextResponse> GetAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}