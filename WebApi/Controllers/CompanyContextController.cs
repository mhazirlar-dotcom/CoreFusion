using Abstractions.Persistence;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[ApiController]
[Route("api/company-context")]
public class CompanyContextController(ICurrentCompanyContext currentCompanyContext , CoreFusionDbContext context) : ControllerBase
{
    #region Fields

    private readonly ICurrentCompanyContext _currentCompanyContext = currentCompanyContext;
    private readonly CoreFusionDbContext _context = context;

    #endregion Fields

    #region Methods

    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
    {
        string databaseName = _context.Database.GetDbConnection().Database;

        bool canConnect = await _context.Database.CanConnectAsync(cancellationToken);

        return Ok(new
        {
            _currentCompanyContext.CompanyId ,
            ExpectedDatabaseName = _currentCompanyContext.DatabaseName ,
            ConnectedDatabaseName = databaseName ,
            CanConnect = canConnect
        });
    }

    #endregion Methods
}