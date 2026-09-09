using Domain.Common;
using Domain.Master.Definitions;

namespace Domain.Master.Companies;

public class CompanyContact : Entity<Guid>
{
    #region Properties

    public Guid CompanyId { get; set; }

    public Guid ContactTypeId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string MobilePhone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsAuthorized { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;

    public ContactType ContactType { get; set; } = null!;

    #endregion Properties
}