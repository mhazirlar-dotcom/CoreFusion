using Domain.Master.Tax;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class TaxOfficeConfiguration : IEntityTypeConfiguration<TaxOffice>
{
    #region Methods

    public void Configure(EntityTypeBuilder<TaxOffice> builder)
    {
        builder.ToTable(TableNames.TaxOffices);

        builder.HasKey(taxOffice => taxOffice.Id);

        builder.Property(taxOffice => taxOffice.Id)
            .ValueGeneratedNever();

        builder.Property(taxOffice => taxOffice.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(taxOffice => taxOffice.Code)
            .HasMaxLength(50);

        builder.Property(taxOffice => taxOffice.CityId)
            .IsRequired();

        builder.Property(taxOffice => taxOffice.IsActive)
            .IsRequired();

        builder.HasOne(taxOffice => taxOffice.City)
            .WithMany()
            .HasForeignKey(taxOffice => taxOffice.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(taxOffice => taxOffice.Companies)
            .WithOne(company => company.TaxOffice)
            .HasForeignKey(company => company.TaxOfficeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}