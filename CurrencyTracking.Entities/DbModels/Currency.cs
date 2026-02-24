using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTracking.Entities.DbModels;

public class Currency
{
	[Column("id")]
	public int Id { get; set; }

	[Column("name")]
	public string Name { get; set; } = string.Empty;

	[Column("rate")]
	public decimal Rate { get; set; }
}
