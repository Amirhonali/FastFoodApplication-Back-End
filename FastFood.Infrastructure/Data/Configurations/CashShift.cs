using FastFood.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFood.Infrastructure.Data.Configurations
{
    public class CashShiftConfiguration : IEntityTypeConfiguration<CashShift>
    {
        public void Configure(EntityTypeBuilder<CashShift> builder)
        {
            builder.ToTable("CashShifts");

            builder.Property(cs => cs.OpeningAmount).HasColumnType("decimal(18,2)");
            builder.Property(cs => cs.ClosingAmount).HasColumnType("decimal(18,2)");

            builder.HasOne(cs => cs.Cashier)
                .WithMany(s => s.CashShifts)
                .HasForeignKey(cs => cs.CashierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cs => cs.Branch)
                .WithMany(b => b.CashShifts)
                .HasForeignKey(cs => cs.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}