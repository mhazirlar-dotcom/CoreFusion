using Abstractions.Infrastructure.Persistence.Repositories;
using Abstractions.Persistence;
using Domain.Master.Companies;

namespace WebApi.Middleware;

public class CompanyContextMiddleware(RequestDelegate next)
{
    #region Fields

    private const string CompanyHeaderName = "X-Company-Id";

    private readonly RequestDelegate _next = next;

    #endregion Fields

    #region Methods

    public async Task InvokeAsync(HttpContext httpContext , ICurrentCompanyContext currentCompanyContext , IMasterRepository<Company , Guid> companyRepository)
    {
        if (!httpContext.Request.Headers.TryGetValue(CompanyHeaderName , out Microsoft.Extensions.Primitives.StringValues companyHeader))
        {
            await _next(httpContext);

            return;
        }

        if (!Guid.TryParse(companyHeader.ToString() , out Guid companyId) || companyId == Guid.Empty)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(new
            {
                Message = "Geçerli bir CompanyId gönderilmelidir."
            });

            return;
        }

        Company company = await companyRepository.GetAsync(
            item => item.Id == companyId && item.IsActive,
            httpContext.RequestAborted);

        currentCompanyContext.SetCompany(company.Id , company.DatabaseName);

        await _next(httpContext);
    }

    #endregion Methods
}