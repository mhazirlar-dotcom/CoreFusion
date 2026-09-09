using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Abstractions.Core.Results;
using Microsoft.AspNetCore.Mvc;
using IResult = Abstractions.Core.Results.IResult;

namespace WebApi.Controllers.Master;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController(ICompanyService companyService) : ControllerBase
{
    #region Fields

    private readonly ICompanyService _companyService = companyService;

    #endregion Fields

    #region Methods

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCompanyRequest request , CancellationToken cancellationToken)
    {
        IDataResult<Guid> result = await _companyService.CreateAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        IDataResult<List<CompanyModel>> result = await _companyService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id , CancellationToken cancellationToken)
    {
        IDataResult<CompanyModel> result = await _companyService.GetByIdAsync(id, cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(UpdateCompanyRequest request , CancellationToken cancellationToken)
    {
        IResult result = await _companyService.UpdateAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id , CancellationToken cancellationToken)
    {
        IResult result = await _companyService.DeleteAsync(id, cancellationToken);

        return Ok(result);
    }

    #endregion Methods
}