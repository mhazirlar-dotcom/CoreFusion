using System.Net.Http;
using Wpf.State;

namespace Wpf.Http;

public class ActiveCompanyDelegatingHandler(WorkingContext workingContext) : DelegatingHandler
{
    #region Fields

    private readonly WorkingContext _workingContext = workingContext;

    #endregion Fields

    #region Methods

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request , CancellationToken cancellationToken)
    {
        Guid? companyId = _workingContext.ActiveCompany?.Id;

        if (companyId.HasValue && companyId.Value != Guid.Empty)
        {
            request.Headers.Remove("X-Company-Id");
            request.Headers.Add("X-Company-Id" , companyId.Value.ToString());
        }

        return base.SendAsync(request , cancellationToken);
    }

    #endregion Methods
}