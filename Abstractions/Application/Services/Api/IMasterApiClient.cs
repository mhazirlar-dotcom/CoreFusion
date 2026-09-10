using Abstractions.Application.Models.Master;

namespace Abstractions.Application.Services.Api;

public interface IMasterApiClient : IApiClient
{
    #region Methods

    Task<List<MasterDefinitionResponse>> GetAddressTypesAsync(CancellationToken cancellationToken = default);
    Task<List<MasterDefinitionResponse>> GetContactTypesAsync(CancellationToken cancellationToken = default);
    Task<List<MasterDefinitionResponse>> GetTaxOfficesAsync(CancellationToken cancellationToken = default);
    Task<List<CityResponse>> GetCitiesAsync(CancellationToken cancellationToken = default);
    Task<List<DistrictResponse>> GetDistrictsAsync(Guid cityId , CancellationToken cancellationToken = default);

    #endregion Methods
}