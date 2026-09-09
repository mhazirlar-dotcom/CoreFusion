using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.Results;
using Core.Results;
using System.Net.Http;
using System.Net.Http.Json;

namespace Wpf.Api;

public class CompanyApiClient(HttpClient httpClient) : ICompanyApiClient
{
    #region Fields

    private readonly HttpClient _httpClient = httpClient;

    #endregion Fields

    #region Methods

    public async Task<IDataResult<Guid>> CreateAsync(CreateCompanyRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/companies", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma oluşturulamadı. HTTP {(int)response.StatusCode}."
                    : message);
        }

        DataResult<Guid>? result = await response.Content.ReadFromJsonAsync<DataResult<Guid>>(cancellationToken);

        return result ?? throw new InvalidOperationException("API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IDataResult<List<CompanyModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync("api/companies", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firmalar alınamadı. HTTP {(int)response.StatusCode}."
                    : message);
        }

        DataResult<List<CompanyModel>>? result = await response.Content.ReadFromJsonAsync<DataResult<List<CompanyModel>>>(cancellationToken);

        return result ?? throw new InvalidOperationException("API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IDataResult<CompanyModel>> GetByIdAsync(Guid id , CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/companies/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma alınamadı. HTTP {(int)response.StatusCode}."
                    : message);
        }

        DataResult<CompanyModel>? result = await response.Content.ReadFromJsonAsync<DataResult<CompanyModel>>(cancellationToken);

        return result ?? throw new InvalidOperationException("API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IResult> UpdateAsync(UpdateCompanyRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync("api/companies", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma güncellenemedi. HTTP {(int)response.StatusCode}."
                    : message);
        }

        Result? result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken);

        return result ?? throw new InvalidOperationException("API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IResult> DeleteAsync(Guid id , CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"api/companies/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma silinemedi. HTTP {(int)response.StatusCode}."
                    : message);
        }

        Result? result = await response.Content.ReadFromJsonAsync<Result>(cancellationToken);

        return result ?? throw new InvalidOperationException("API geçerli bir sonuç döndürmedi.");
    }

    #endregion Methods
}