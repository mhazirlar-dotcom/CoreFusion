using Abstractions.Application.Models.Master;

namespace Abstractions.Application.Services.Master;

public interface ILocationService
{
    #region Methods

    Task<List<CityResponse>> GetCitiesAsync(CancellationToken cancellationToken = default);

    Task<List<DistrictResponse>> GetDistrictsAsync(Guid cityId , CancellationToken cancellationToken = default);

    #endregion Methods
}