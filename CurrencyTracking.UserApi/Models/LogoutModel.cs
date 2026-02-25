namespace CurrencyTracking.UserApi.Models;

public record LogoutModel
{
	public string RefreshToken { get; init; }
}
