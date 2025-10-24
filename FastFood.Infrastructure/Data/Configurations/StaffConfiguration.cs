using FastFood.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFood.Infrastructure.Data.Configurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staffs");

            builder.HasIndex(s => s.Username).IsUnique();

            builder.Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(s => s.Role)
                .WithMany(r => r.StaffMembers)
                .HasForeignKey(s => s.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Branch)
                .WithMany(b => b.StaffMembers)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}