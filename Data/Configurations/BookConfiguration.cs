using LibraryManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Web.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.BookId);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(250);
        builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);

        // ISBN must be unique across the catalog
        builder.HasIndex(b => b.ISBN).IsUnique();

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.BorrowRecords)
            .WithOne(br => br.Book)
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}