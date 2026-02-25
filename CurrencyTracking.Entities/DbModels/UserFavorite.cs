using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTracking.Entities.DbModels;

[PrimaryKey(nameof(UserId), nameof(CurrencyId))]
public class UserFavorite
{
	[Column("user_id")]
	public Guid UserId { get; set; }
	public User User { get; set; }

	[Column("currency_id")]
	public string CurrencyId { get; set; }
	public Currency Currency { get; set; }
}
