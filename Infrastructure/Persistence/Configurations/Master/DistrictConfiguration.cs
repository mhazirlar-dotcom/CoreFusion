using Domain.Master.Locations;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    #region Methods

    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable(TableNames.Districts);

        builder.HasKey(district => district.Id);

        builder.Property(district => district.Id)
            .ValueGeneratedNever();

        builder.Property(district => district.CityId)
            .IsRequired();

        builder.Property(district => district.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(district => district.IsActive)
            .IsRequired();

        builder.HasOne(district => district.City)
            .WithMany(city => city.Districts)
            .HasForeignKey(district => district.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(district => district.CompanyAddresses)
            .WithOne(companyAddress => companyAddress.District)
            .HasForeignKey(companyAddress => companyAddress.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}