using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Master;

[ApiController]
[Route("api/master/definitions")]
public class MasterDefinitionsController(IMasterDefinitionService masterDefinitionService) : ControllerBase
{
    #region Fields

    private readonly IMasterDefinitionService _masterDefinitionService = masterDefinitionService;

    #endregion Fields

    #region Methods

    [HttpGet("address-types")]
    public async Task<ActionResult<List<MasterDefinitionResponse>>> GetAddressTypesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _masterDefinitionService.GetAddressTypesAsync(cancellationToken));
    }

    [HttpGet("contact-types")]
    public async Task<ActionResult<List<MasterDefinitionResponse>>> GetContactTypesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _masterDefinitionService.GetContactTypesAsync(cancellationToken));
    }

    [HttpGet("tax-offices")]
    public async Task<ActionResult<List<MasterDefinitionResponse>>> GetTaxOfficesAsync(CancellationToken cancellationToken)
    {
        return Ok(await _masterDefinitionService.GetTaxOfficesAsync(cancellationToken));
    }

    #endregion Methods
}