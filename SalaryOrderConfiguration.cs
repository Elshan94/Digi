using DigitalSalaryService.Persistence.Constants;
using DigitalSalaryService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalSalaryService.Persistence.Configurations
{
    public class SalaryOrderConfiguration : IEntityTypeConfiguration<SalaryOrder>
    {
        public void Configure(EntityTypeBuilder<SalaryOrder> builder)
        {
            builder.HasIndex(m => m.RequestId).IsUnique();

            builder.Property(m => m.RequestId).HasMaxLength(64).IsRequired();
            builder.Property(m => m.CustomerCode).HasMaxLength(10).IsRequired();
            builder.Property(m => m.CallbackStatus).HasMaxLength(20);
            builder.Property(m => m.ProfileId).HasMaxLength(50);
            builder.Property(m => m.Iban).HasMaxLength(50);
            builder.Property(m => m.FileName).HasMaxLength(50);
            builder.Property(m => m.DocumentNumber).HasMaxLength(25);
            builder.Property(m => m.PersonalNumber).HasMaxLength(15);
            builder.Property(m => m.KycId).HasMaxLength(50);
            builder.Property(m => m.FatcaId).HasMaxLength(50);
            builder.Property(m => m.CurrentStep).IsRequired(true).HasConversion(m => m.Name, x => CurrentStep.FromName(x, false)).HasMaxLength(50);

            builder.Property(m => m.ApplicationId).HasMaxLength(50);
            builder.Property(m => m.PartnerId).HasMaxLength(50);

            builder.Property(m => m.Name).HasMaxLength(50);
            builder.Property(m => m.Surname).HasMaxLength(50);
            builder.Property(m => m.Patronymic).HasMaxLength(50);
        }
    }
}
