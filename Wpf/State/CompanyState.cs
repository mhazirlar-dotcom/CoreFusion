using Abstractions.Application.Models.Master;
using System.Collections.ObjectModel;

namespace Wpf.State;

public class CompanyState
{
    #region Properties

    public ObservableCollection<CompanyModel> Companies { get; } = [];

    #endregion Properties

    #region Methods

    public void SetCompanies(IEnumerable<CompanyModel> companies)
    {
        ArgumentNullException.ThrowIfNull(companies);

        Companies.Clear();

        foreach (CompanyModel company in companies)
        {
            Companies.Add(company);
        }
    }

    public void Add(CompanyModel company)
    {
        ArgumentNullException.ThrowIfNull(company);

        Companies.Add(company);
    }

    public void Remove(Guid companyId)
    {
        CompanyModel? company = Companies.FirstOrDefault(item => item.Id == companyId);

        if (company is not null)
        {
            Companies.Remove(company);
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
                return;
            }
        }
    }

    #endregion Methods
}