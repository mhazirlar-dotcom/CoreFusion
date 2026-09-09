using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Finance;

public class FinancialAccount : Entity<Guid>
{
    #region Properties

    public Guid CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public decimal Balance { get; set; }

    #endregion Properties
}