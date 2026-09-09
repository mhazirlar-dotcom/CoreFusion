using Domain.Master.Companies;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class CompanyContactConfiguration : IEntityTypeConfiguration<CompanyContact>
{
    #region Methods

    public void Configure(EntityTypeBuilder<CompanyContact> builder)
    {
        builder.ToTable(TableNames.CompanyContacts);

        builder.HasKey(companyContact => companyContact.Id);

        builder.Property(companyContact => companyContact.Id)
            .ValueGeneratedNever();

        builder.Property(companyContact => companyContact.CompanyId)
            .IsRequired();

        builder.Property(companyContact => companyContact.ContactTypeId)
            .IsRequired();

        builder.Property(companyContact => companyContact.FirstName)
            .HasMaxLength(100);

        builder.Property(companyContact => companyContact.LastName)
            .HasMaxLength(100);

        builder.Property(companyContact => companyContact.Title)
            .HasMaxLength(150);

        builder.Property(companyContact => companyContact.Phone)
            .HasMaxLength(30);

        builder.Property(companyContact => companyContact.MobilePhone)
            .HasMaxLength(30);

        builder.Property(companyContact => companyContact.Email)
            .HasMaxLength(200);

        builder.Property(companyContact => companyContact.IsAuthorized)
            .IsRequired();

        builder.Property(companyContact => companyContact.IsDefault)
            .IsRequired();

        builder.Property(companyContact => companyContact.IsActive)
            .IsRequired();

        builder.HasOne(companyContact => companyContact.Company)
            .WithMany(company => company.CompanyContacts)
            .HasForeignKey(companyContact => companyContact.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(companyContact => companyContact.ContactType)
            .WithMany(contactType => contactType.CompanyContacts)
            .HasForeignKey(companyContact => companyContact.ContactTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    #endregion Methods
}