using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTracking.Entities.DbModels;

[Table("users")]
public class User
{
	[Column("id")]
	public Guid Id { get; set; }

	[Column("name")]
	public string Name { get; set; } = string.Empty;

	[Column("password")]
	public string PasswordHash { get; set; } = string.Empty;

	public IQueryable<UserFavorite> Favorites { get; set; }
}
