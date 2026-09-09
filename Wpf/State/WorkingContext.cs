using Abstractions.Application.Models.Master;
using Abstractions.Core.DependencyInjection;

namespace Wpf.State;

public class WorkingContext: ISingletonService
{
    #region Properties

    public CompanyModel? ActiveCompany { get; private set; }

    public CompanyPeriodModel? ActivePeriod { get; private set; }

    #endregion Properties

    #region Events

    public event EventHandler? ContextChanged;

    #endregion Events

    #region Methods

    public void SetCompany(CompanyModel company)
    {
        ArgumentNullException.ThrowIfNull(company);

        ActiveCompany = company;
        ActivePeriod = null;

        ContextChanged?.Invoke(this , EventArgs.Empty);
    }

    public void SetPeriod(CompanyPeriodModel period)
    {
        ArgumentNullException.ThrowIfNull(period);

        if (ActiveCompany is null)
        {
            throw new InvalidOperationException("Önce aktif firma seçilmelidir.");
        }

        if (period.CompanyId != ActiveCompany.Id)
        {
            throw new InvalidOperationException("Seçilen dönem aktif firmaya ait değildir.");
        }

        ActivePeriod = period;

        ContextChanged?.Invoke(this , EventArgs.Empty);
    }

    public void Clear()
    {
        ActiveCompany = null;
        ActivePeriod = null;

        ContextChanged?.Invoke(this , EventArgs.Empty);
    }

    #endregion Methods
}