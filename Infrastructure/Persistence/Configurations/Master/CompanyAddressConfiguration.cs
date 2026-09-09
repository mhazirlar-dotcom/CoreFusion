using Domain.Master.Companies;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class CompanyAddressConfiguration : IEntityTypeConfiguration<CompanyAddress>
{
    #region Methods

    public void Configure(EntityTypeBuilder<CompanyAddress> builder)
    {
        builder.ToTable(TableNames.CompanyAddresses);

        builder.HasKey(companyAddress => companyAddress.Id);

        builder.Property(companyAddress => companyAddress.Id)
            .ValueGeneratedNever();

        builder.Property(companyAddress => companyAddress.CompanyId)
            .IsRequired();

        builder.Property(companyAddress => companyAddress.CityId)
            .IsRequired();

        builder.Property(companyAddress => companyAddress.DistrictId)
            .IsRequired();

        builder.Property(companyAddress => companyAddress.AddressTypeId)
            .IsRequired();

        builder.Property(companyAddress => companyAddress.Address)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(companyAddress => companyAddress.PostalCode)
            .HasMaxLength(20);

        builder.Property(companyAddress => companyAddress.IsDefault)
            .IsRequired();

        builder.HasOne(companyAddress => companyAddress.Company)
            .WithMany(company => company.CompanyAddresses)
            .HasForeignKey(companyAddress => companyAddress.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(companyAddress => companyAddress.City)
            .WithMany(city => city.CompanyAddresses)
            .HasForeignKey(companyAddress => companyAddress.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(companyAddress => companyAddress.District)
            .WithMany(district => district.CompanyAddresses)
            .HasForeignKey(companyAddress => companyAddress.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(companyAddress => companyAddress.AddressType)
            .WithMany(addressType => addressType.CompanyAddresses)
            .HasForeignKey(companyAddress => companyAddress.AddressTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}