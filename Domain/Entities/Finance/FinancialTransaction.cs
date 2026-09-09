using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Finance;

public class FinancialTransaction : Entity<Guid>
{
    #region Properties

    public Guid CompanyId { get; set; }

    public Guid FinancialAccountId { get; set; }

    public TransactionType Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    #endregion Properties
}