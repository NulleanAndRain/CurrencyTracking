using Microsoft.Extensions.Configuration;

namespace CurrencyTracking.UserService.Utils;

public static class KeycloakConfigUtils
{
	public const string KeycloakSectionName = "Keycloak";

	public static string GetClientId(this IConfiguration configuration)
	{
		var section = GetSection(configuration);
		return section["resource"]!;
	}
	
	public static string GetKeycloakLoginUrl(this IConfiguration configuration)
	{
		var section = GetSection(configuration);
		var baseUrl = section["auth-server-url"];
		var realm = section["realm"];
		return $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";
	}

	public static string GetKeycloakRealm(this IConfiguration configuration) => configuration.GetSection(KeycloakSectionName)["realm"]!;

	public static string GetKeycloakLogoutUrl(this IConfiguration configuration)
	{
		var section = GetSection(configuration);
		var baseUrl = section["auth-server-url"];
		var realm = section["realm"];
		return $"{baseUrl}/realms/{realm}/protocol/openid-connect/logout";
	}

	private static IConfigurationSection GetSection(IConfiguration configuration) => configuration.GetSection(KeycloakSectionName);
}
