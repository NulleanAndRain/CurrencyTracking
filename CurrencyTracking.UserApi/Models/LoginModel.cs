namespace CurrencyTracking.UserApi.Models;

public record LoginModel
{
	public string Name { get; init; }
	public string Password { get; init; }
}
