using Domain.Master.Companies;
using Infrastructure.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Master;

public class CompanyPeriodConfiguration : IEntityTypeConfiguration<CompanyPeriod>
{
    #region Methods

    public void Configure(EntityTypeBuilder<CompanyPeriod> builder)
    {
        builder.ToTable(TableNames.CompanyPeriods);

        builder.HasKey(companyPeriod => companyPeriod.Id);

        builder.Property(companyPeriod => companyPeriod.Id)
            .ValueGeneratedNever();

        builder.Property(companyPeriod => companyPeriod.CompanyId)
            .IsRequired();

        builder.Property(companyPeriod => companyPeriod.StartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(companyPeriod => companyPeriod.EndDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(companyPeriod => companyPeriod.IsActive)
            .IsRequired();

        builder.HasOne(companyPeriod => companyPeriod.Company)
            .WithMany(company => company.CompanyPeriods)
            .HasForeignKey(companyPeriod => companyPeriod.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(companyPeriod => new
        {
            companyPeriod.CompanyId ,
            companyPeriod.StartDate ,
            companyPeriod.EndDate
        });
    }

    #endregion Methods
}