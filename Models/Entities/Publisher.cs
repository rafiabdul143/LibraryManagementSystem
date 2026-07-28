using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.Entities;

public class Publisher
{
	public int PublisherId { get; set; }

	[Required, StringLength(150)]
	public string Name { get; set; } = string.Empty;

	[StringLength(300)]
	public string? Address { get; set; }

	public ICollection<Book> Books { get; set; } = new List<Book>();
}