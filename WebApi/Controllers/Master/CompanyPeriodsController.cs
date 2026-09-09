using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Abstractions.Core.Results;
using Microsoft.AspNetCore.Mvc;
using IResult = Abstractions.Core.Results.IResult;

namespace WebApi.Controllers.Master;

[ApiController]
[Route("api/companies/{companyId:guid}/periods")]
public class CompanyPeriodsController(ICompanyPeriodService companyPeriodService) : ControllerBase
{
    #region Fields

    private readonly ICompanyPeriodService _companyPeriodService = companyPeriodService;

    #endregion Fields

    #region Methods

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid companyId , CreateCompanyPeriodRequest request , CancellationToken cancellationToken)
    {
        CreateCompanyPeriodRequest createRequest = request with
        {
            CompanyId = companyId
        };

        IDataResult<Guid> result = await _companyPeriodService.CreateAsync(
            createRequest,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCompanyIdAsync(Guid companyId , CancellationToken cancellationToken)
    {
        IDataResult<List<CompanyPeriodModel>> result = await _companyPeriodService.GetByCompanyIdAsync(
            companyId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid companyId , Guid id , CancellationToken cancellationToken)
    {
        IDataResult<CompanyPeriodModel> result = await _companyPeriodService.GetByIdAsync(
            companyId,
            id,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid companyId , Guid id , UpdateCompanyPeriodRequest request , CancellationToken cancellationToken)
    {
        UpdateCompanyPeriodRequest updateRequest = request with
        {
            Id = id
        };

        IResult result = await _companyPeriodService.UpdateAsync(
            companyId,
            updateRequest,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid companyId , Guid id , CancellationToken cancellationToken)
    {
        IResult result = await _companyPeriodService.DeleteAsync(
            companyId,
            id,
            cancellationToken);

        return Ok(result);
    }

    #endregion Methods
}