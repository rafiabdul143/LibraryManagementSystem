using LibraryManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Web.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.MemberId);

        builder.Property(m => m.FullName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(m => m.Email)
            .IsUnique();

        builder.Property(m => m.Phone)
            .HasMaxLength(20);

        builder.Property(m => m.MembershipDate)
            .IsRequired();

        // Optional one-to-one relationship with ApplicationUser
        builder.HasOne(m => m.ApplicationUser)
            .WithOne(u => u.Member)
            .HasForeignKey<Member>(m => m.ApplicationUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // One Member -> Many BorrowRecords
        builder.HasMany(m => m.BorrowRecords)
            .WithOne(br => br.Member)
            .HasForeignKey(br => br.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}