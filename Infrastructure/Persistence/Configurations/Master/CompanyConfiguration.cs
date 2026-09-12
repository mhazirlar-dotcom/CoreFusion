using Domain.Master.Companies;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    #region Methods

    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable(TableNames.Companies);

        builder.HasKey(company => company.Id);

        builder.Property(company => company.Id)
            .ValueGeneratedNever();

        builder.Property(company => company.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(company => company.ShortName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(company => company.TaxNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(company => company.TcIdentityNumber)
            .HasMaxLength(11);

        builder.Property(company => company.EstablishmentDate)
            .IsRequired();

        builder.Property(company => company.ClosingDate);

        builder.Property(company => company.TradeRegistryNumber)
            .HasMaxLength(50);

        builder.Property(company => company.MersisNumber)
            .HasMaxLength(20);

        builder.Property(company => company.Website)
            .HasMaxLength(500);

        builder.Property(company => company.LogoPath)
            .HasMaxLength(500);

        builder.Property(company => company.DatabaseName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(company => company.IsActive)
            .IsRequired();

        builder.HasOne(company => company.TaxOffice)
            .WithMany(taxOffice => taxOffice.Companies)
            .HasForeignKey(company => company.TaxOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(company => company.CompanyAddresses)
            .WithOne(companyAddress => companyAddress.Company)
            .HasForeignKey(companyAddress => companyAddress.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(company => company.CompanyContacts)
            .WithOne(companyContact => companyContact.Company)
            .HasForeignKey(companyContact => companyContact.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    #endregion Methods
}