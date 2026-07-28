using LibraryManagement.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Web.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
	public void Configure(EntityTypeBuilder<Author> builder)
	{
		builder.ToTable("Authors");
		builder.HasKey(a => a.AuthorId);

		builder.Property(a => a.FullName).IsRequired().HasMaxLength(150);
		builder.Property(a => a.Bio).HasMaxLength(1000);

		// One author -> many books. Restrict prevents deleting an author
		// that still has books attached (enforced again at the service layer).
		builder.HasMany(a => a.Books)
			.WithOne(b => b.Author)
			.HasForeignKey(b => b.AuthorId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}