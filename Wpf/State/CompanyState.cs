using Abstractions.Application.Models.Master;
using Abstractions.Core.DependencyInjection;
using System.Collections.ObjectModel;

namespace Wpf.State;

public class CompanyState : ISingletonService
{
    #region Properties

    public ObservableCollection<CompanyModel> Companies { get; } = [];

    public CompanyModel? ActiveCompany { get; private set; }

    #endregion Properties

    #region Events

    public event EventHandler<CompanyModel?>? ActiveCompanyChanged;

    #endregion Events

    #region Methods

    public void SetCompanies(IEnumerable<CompanyModel> companies)
    {
        ArgumentNullException.ThrowIfNull(companies);

        Companies.Clear();

        foreach (CompanyModel company in companies)
        {
            Companies.Add(company);
        }

        if (Companies.Count == 0)
        {
            SetActiveCompany(null);
            return;
        }

        if (ActiveCompany is not null)
        {
            CompanyModel? existingCompany = Companies.FirstOrDefault(item => item.Id == ActiveCompany.Id);

            if (existingCompany is not null)
            {
                SetActiveCompany(existingCompany);
                return;
            }
        }

        CompanyModel? activeCompany = Companies.FirstOrDefault(company => company.IsActive);

        SetActiveCompany(activeCompany ?? Companies[0]);
    }

    public void Add(CompanyModel company)
    {
        ArgumentNullException.ThrowIfNull(company);

        Companies.Add(company);
    }

    public void Remove(Guid companyId)
    {
        CompanyModel? company = Companies.FirstOrDefault(item => item.Id == companyId);

        if (company is null)
        {
            return;
        }

        bool isActiveCompany = ActiveCompany?.Id == companyId;

        Companies.Remove(company);

        if (isActiveCompany)
        {
            SetActiveCompany(Companies.FirstOrDefault(item => item.IsActive) ?? Companies.FirstOrDefault());
        }
    }

    public void Update(CompanyModel company)
    {
        ArgumentNullException.ThrowIfNull(company);

        for (int index = 0 ; index < Companies.Count ; index++)
        {
            if (Companies[index].Id == company.Id)
            {
                Companies[index] = company;

                if (ActiveCompany?.Id == company.Id)
                {
                    SetActiveCompany(company);
                }

                return;
            }
        }
    }

    public void SetActiveCompany(CompanyModel? company)
    {
        if (company is not null && !Companies.Any(item => item.Id == company.Id))
        {
            throw new InvalidOperationException("Seçilen firma firma listesinde bulunmuyor.");
        }

        if (ActiveCompany?.Id == company?.Id)
        {
            return;
        }

        ActiveCompany = company;

        ActiveCompanyChanged?.Invoke(this , ActiveCompany);
    }

    #endregion Methods
}