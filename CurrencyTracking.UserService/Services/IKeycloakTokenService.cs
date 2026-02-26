namespace CurrencyTracking.UserService.Services;

public interface IKeycloakTokenService
{
	public Task<string> GetKeycloakTokenAsync();
}
