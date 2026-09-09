using Domain.Common;

namespace Domain.Entities.Commercial;

public class Category : Entity<Guid>
{
    #region Properties
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = [];

    #endregion Properties
}
