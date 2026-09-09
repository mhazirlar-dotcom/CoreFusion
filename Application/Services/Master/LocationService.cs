using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Abstractions.Core.DependencyInjection;
using Abstractions.Infrastructure.Persistence.Repositories;
using Domain.Master.Locations;

namespace Application.Services.Master;

public class LocationService(IMasterRepository<City , Guid> cityRepository , IMasterRepository<District , Guid> districtRepository) : ILocationService, IScopedService
{
    #region Fields

    private readonly IMasterRepository<City, Guid> _cityRepository = cityRepository;
    private readonly IMasterRepository<District, Guid> _districtRepository = districtRepository;

    #endregion Fields

    #region Methods

    public async Task<List<CityResponse>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        List<City> cities = await _cityRepository.GetAllAsync(item => item.IsActive, cancellationToken);

        return [.. cities
            .OrderBy(item => item.Name)
            .Select(item => new CityResponse(item.Id , item.Name , item.Code))];
    }

    public async Task<List<DistrictResponse>> GetDistrictsAsync(Guid cityId , CancellationToken cancellationToken = default)
    {
        List<District> districts = await _districtRepository.GetAllAsync(item => item.CityId == cityId && item.IsActive, cancellationToken);

        return [.. districts
            .OrderBy(item => item.Name)
            .Select(item => new DistrictResponse(item.Id , item.CityId , item.Name))];
    }

    #endregion Methods
}