using Abstractions.Application.Services.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Api;
using Wpf.State;

namespace Wpf.DependencyInjection;

public static class WpfServiceCollectionExtensions
{
    #region Methods

    public static IServiceCollection AddWpfServices(this IServiceCollection services , IConfiguration configuration)
    {
        string baseUrl = configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("ApiSettings:BaseUrl bulunamadı.");

        services.AddHttpClient<IMasterApiClient , MasterApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<ICompanyApiClient , CompanyApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<ICompanyPeriodApiClient , CompanyPeriodApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }

    #endregion Methods
}