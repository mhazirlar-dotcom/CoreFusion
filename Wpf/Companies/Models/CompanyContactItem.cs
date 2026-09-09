namespace Wpf.Companies.Models;

public class CompanyContactItem
{
    #region Properties

    public Guid Id { get; set; }
    public Guid ContactTypeId { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
    public bool IsDefault { get; set; }

    #endregion Properties
}