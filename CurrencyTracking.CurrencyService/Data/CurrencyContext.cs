using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyService.Data;

public class CurrencyContext : DbContext, ICurrencyContext, IDisposable
{
	public DbSet<Currency> Currencies { get; init; }
	public DbSet<UserFavorite> UserFavorites { get; init; }

	public CurrencyContext(DbContextOptions options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserFavorite>()
			.HasKey(e => new { e.UserId, e.CurrencyId });
		base.OnModelCreating(modelBuilder);
	}
}
