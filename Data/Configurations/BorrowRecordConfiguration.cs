using LibraryManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Web.Data.Configurations;

public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
{
    public void Configure(EntityTypeBuilder<BorrowRecord> builder)
    {
        builder.ToTable("BorrowRecords");
        builder.HasKey(br => br.BorrowRecordId);

        builder.Property(br => br.FineAmount)
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0m);

        builder.Property(br => br.Status)
            .HasConversion<string>()   // store enum as readable string, not int
            .HasMaxLength(20);

        // Speeds up dashboard/report queries (overdue lookups, per-member history)
        builder.HasIndex(br => br.Status);
        builder.HasIndex(br => br.DueDate);
    }
}