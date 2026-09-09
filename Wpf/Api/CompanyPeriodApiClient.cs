using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using Core.Results;
using System.Net.Http;
using System.Net.Http.Json;

namespace Wpf.Api;

public class CompanyPeriodApiClient(HttpClient httpClient) : ICompanyPeriodApiClient
{
    #region Fields

    private readonly HttpClient _httpClient = httpClient;

    #endregion Fields

    #region Methods

    public async Task<IDataResult<Guid>> CreateAsync(CreateCompanyPeriodRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            $"api/companies/{request.CompanyId}/periods",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma dönemi oluşturulamadı. HTTP {(int)response.StatusCode}."
                    : message);
        }

        DataResult<Guid>? result = await response.Content.ReadFromJsonAsync<DataResult<Guid>>(
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IDataResult<List<CompanyPeriodModel>>> GetByCompanyIdAsync(Guid companyId , CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"api/companies/{companyId}/periods",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma dönemleri alınamadı. HTTP {(int)response.StatusCode}."
                    : message);
        }

        DataResult<List<CompanyPeriodModel>>? result = await response.Content.ReadFromJsonAsync<DataResult<List<CompanyPeriodModel>>>(
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IDataResult<CompanyPeriodModel>> GetByIdAsync(Guid companyId , Guid id , CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"api/companies/{companyId}/periods/{id}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma dönemi alınamadı. HTTP {(int)response.StatusCode}."
                    : message);
        }

        DataResult<CompanyPeriodModel>? result = await response.Content.ReadFromJsonAsync<DataResult<CompanyPeriodModel>>(
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IResult> UpdateAsync(Guid companyId , UpdateCompanyPeriodRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
            $"api/companies/{companyId}/periods/{request.Id}",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma dönemi güncellenemedi. HTTP {(int)response.StatusCode}."
                    : message);
        }

        Result? result = await response.Content.ReadFromJsonAsync<Result>(
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "API geçerli bir sonuç döndürmedi.");
    }

    public async Task<IResult> DeleteAsync(Guid companyId , Guid id , CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(
            $"api/companies/{companyId}/periods/{id}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Firma dönemi silinemedi. HTTP {(int)response.StatusCode}."
                    : message);
        }

        Result? result = await response.Content.ReadFromJsonAsync<Result>(
            cancellationToken);

        return result ?? throw new InvalidOperationException(
            "API geçerli bir sonuç döndürmedi.");
    }

    #endregion Methods
}