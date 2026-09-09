using Domain.Common;

namespace Domain.Entities.Commercial;

public class Product : Entity<Guid>
{
    #region Properties
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    #endregion Properties
}
