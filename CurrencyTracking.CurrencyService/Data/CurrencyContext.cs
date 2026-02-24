using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyService.Data;

public class CurrencyContext : DbContext, ICurrencyContext
{
	public DbSet<Currency> Currencies { get; init; }
	public DbSet<UserFavorite> UserFavorites { get; init; }
}
