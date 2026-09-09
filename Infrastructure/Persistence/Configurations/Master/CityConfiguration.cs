using Domain.Master.Locations;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    #region Methods

    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable(TableNames.Cities);

        builder.HasKey(city => city.Id);

        builder.Property(city => city.Id)
            .ValueGeneratedNever();

        builder.Property(city => city.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(city => city.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(city => city.IsActive)
            .IsRequired();

        builder.HasMany(city => city.Districts)
            .WithOne(district => district.City)
            .HasForeignKey(district => district.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(city => city.CompanyAddresses)
            .WithOne(companyAddress => companyAddress.City)
            .HasForeignKey(companyAddress => companyAddress.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}