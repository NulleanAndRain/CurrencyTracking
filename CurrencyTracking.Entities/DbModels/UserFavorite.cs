using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTracking.Entities.DbModels;

[Index(nameof(UserId), nameof(CurrencyId), IsUnique = true)]
public class UserFavorite
{
	[Column("user_id")]
	public int UserId { get; set; }
	public User User { get; set; }

	[Column("currency_id")]
	public int CurrencyId { get; set; }
	public Currency Currency { get; set; }
}
