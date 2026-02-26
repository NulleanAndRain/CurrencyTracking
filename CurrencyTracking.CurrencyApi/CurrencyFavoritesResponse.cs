using CurrencyTracking.Entities.DbModels;

namespace CurrencyTracking.CurrencyApi;

public class CurrencyFavoritesResponse
{
	public IEnumerable<Currency> Data { get; set; }
}
