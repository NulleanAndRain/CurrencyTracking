using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyService.Data;

public interface ICurrencyContext
{
	DbSet<Currency> Currencies { get; }
	DbSet<UserFavorite> UserFavorites { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
