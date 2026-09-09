using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace Wpf.Api;

public class MasterApiClient(HttpClient httpClient) : IMasterApiClient
{
    #region Fields

    private readonly HttpClient _httpClient = httpClient;

    #endregion Fields

    #region Methods

    public async Task<List<MasterDefinitionResponse>> GetAddressTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<MasterDefinitionResponse>>("api/master/definitions/address-types" , cancellationToken) ?? [];
    }

    public async Task<List<MasterDefinitionResponse>> GetContactTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<MasterDefinitionResponse>>("api/master/definitions/contact-types" , cancellationToken) ?? [];
    }

    public async Task<List<MasterDefinitionResponse>> GetTaxOfficesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<MasterDefinitionResponse>>("api/master/definitions/tax-offices" , cancellationToken) ?? [];
    }

    public async Task<List<CityResponse>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<CityResponse>>("api/master/locations/cities" , cancellationToken) ?? [];
    }

    public async Task<List<DistrictResponse>> GetDistrictsAsync(Guid cityId , CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<DistrictResponse>>($"api/master/locations/cities/{cityId}/districts" , cancellationToken) ?? [];
    }

    #endregion Methods
}