using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Master;

[ApiController]
[Route("api/master/locations")]
public class LocationsController(ILocationService locationService) : ControllerBase
{
    #region Fields

    private readonly ILocationService _locationService = locationService;

    #endregion Fields

    #region Methods

    [HttpGet("cities")]
    public async Task<ActionResult<List<CityResponse>>> GetCitiesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _locationService.GetCitiesAsync(cancellationToken));
    }

    [HttpGet("cities/{cityId:guid}/districts")]
    public async Task<ActionResult<List<DistrictResponse>>> GetDistrictsAsync(Guid cityId , CancellationToken cancellationToken)
    {
        return Ok(await _locationService.GetDistrictsAsync(cityId , cancellationToken));
    }

    #endregion Methods
}