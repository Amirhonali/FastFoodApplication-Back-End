using FastFood.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFood.Infrastructure.Data.Configurations
{
    public class CashTransactionConfiguration : IEntityTypeConfiguration<CashTransaction>
    {
        public void Configure(EntityTypeBuilder<CashTransaction> builder)
        {
            builder.ToTable("CashTransactions");

            builder.Property(ct => ct.Amount).HasColumnType("decimal(18,2)");
            builder.Property(ct => ct.Type).IsRequired().HasMaxLength(20);
            builder.Property(ct => ct.Category).IsRequired().HasMaxLength(50);
            builder.Property(ct => ct.Description).HasMaxLength(255);

            builder.HasOne(ct => ct.Shift)
                .WithMany(cs => cs.Transactions)
                .HasForeignKey(ct => ct.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ct => ct.CreatedBy)
                .WithMany()
                .HasForeignKey(ct => ct.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}