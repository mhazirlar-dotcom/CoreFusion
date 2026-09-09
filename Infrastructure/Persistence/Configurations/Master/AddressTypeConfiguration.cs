using Domain.Master.Definitions;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class AddressTypeConfiguration : IEntityTypeConfiguration<AddressType>
{
    #region Methods

    public void Configure(EntityTypeBuilder<AddressType> builder)
    {
        builder.ToTable(TableNames.AddressTypes);

        builder.HasKey(addressType => addressType.Id);

        builder.Property(addressType => addressType.Id)
            .ValueGeneratedNever();

        builder.Property(addressType => addressType.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(addressType => addressType.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(addressType => addressType.IsActive)
            .IsRequired();

        builder.HasMany(addressType => addressType.CompanyAddresses)
            .WithOne(companyAddress => companyAddress.AddressType)
            .HasForeignKey(companyAddress => companyAddress.AddressTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}