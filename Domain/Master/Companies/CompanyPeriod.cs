using Domain.Common;

namespace Domain.Master.Companies;

public class CompanyPeriod : Entity<Guid>
{
    #region Properties

    public Guid CompanyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;

    #endregion Properties
}