using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurrencyTracking.Entities.DbModels;

[Table("currencies")]
public class Currency
{
	[Column("id")]
	public string Id { get; set; }

	[Column("name")]
	[MaxLength(255)]
	public string Name { get; set; } = string.Empty;

	[Column("rate")]
	public decimal Rate { get; set; }
}
