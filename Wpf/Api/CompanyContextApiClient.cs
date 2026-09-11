using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace Wpf.Api;

public class CompanyContextApiClient(HttpClient httpClient) : ICompanyContextApiClient
{
    #region Fields

    private readonly HttpClient _httpClient = httpClient;

    #endregion Fields

    #region Methods

    public async Task<CompanyContextResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        CompanyContextResponse? response = await _httpClient.GetFromJsonAsync<CompanyContextResponse>(
            "api/company-context",
            cancellationToken);

        return response ?? throw new InvalidOperationException("Company context API boş cevap döndürdü.");
    }

    #endregion Methods
}