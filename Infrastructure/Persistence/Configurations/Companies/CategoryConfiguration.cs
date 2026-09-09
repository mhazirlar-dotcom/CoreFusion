using Domain.Entities.Commercial;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Companies;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    #region Methods

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(TableNames.Categories);

        builder.HasKey(category => category.Id);

        builder.Property(category => category.Id)
            .ValueGeneratedNever();

        builder.Property(category => category.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(category => category.Products)
            .WithOne(product => product.Category)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}