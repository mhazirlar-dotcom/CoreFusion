using Domain.Master.Definitions;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class ContactTypeConfiguration : IEntityTypeConfiguration<ContactType>
{
    #region Methods

    public void Configure(EntityTypeBuilder<ContactType> builder)
    {
        builder.ToTable(TableNames.ContactTypes);

        builder.HasKey(contactType => contactType.Id);

        builder.Property(contactType => contactType.Id)
            .ValueGeneratedNever();

        builder.Property(contactType => contactType.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(contactType => contactType.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(contactType => contactType.IsActive)
            .IsRequired();

        builder.HasMany(contactType => contactType.CompanyContacts)
            .WithOne(companyContact => companyContact.ContactType)
            .HasForeignKey(companyContact => companyContact.ContactTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}