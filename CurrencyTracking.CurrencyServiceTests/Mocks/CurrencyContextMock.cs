using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyServiceTests.Mocks;

public class CurrencyContextMock : DbContext, ICurrencyContext
{

	public DbSet<Currency> Currencies { get; set; }

	public DbSet<UserFavorite> UserFavorites { get; set; }

	public CurrencyContextMock(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserFavorite>()
			.HasKey(e => new { e.UserId, e.CurrencyId });
		base.OnModelCreating(modelBuilder);
	}
}
