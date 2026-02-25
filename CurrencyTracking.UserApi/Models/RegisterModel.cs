namespace CurrencyTracking.UserApi.Models;

public record RegisterModel
{
	public string Name { get; init; }
	public string Password { get; init; }
}
