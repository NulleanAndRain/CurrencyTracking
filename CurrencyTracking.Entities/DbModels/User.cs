using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTracking.Entities.DbModels;

public class User
{
	[Column("id")]
	public int Id { get; set; }

	[Column("name")]
	public string Name { get; set; } = string.Empty;

	[Column("password")]
	public string PasswordHash {  get; set; }= string.Empty;

	public IQueryable<UserFavorite> Favorites { get; set; }
}
