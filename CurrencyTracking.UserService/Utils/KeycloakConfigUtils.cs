using Microsoft.Extensions.Configuration;

namespace CurrencyTracking.UserService.Utils;

public static class KeycloakConfigUtils
{
	public const string KeycloakSectionName = "Keycloak";

	public static string GetClientId(this IConfiguration configuration)
	{
		var section = GetKeycloakSection(configuration);
		return section["resource"]!;
	}

	public static string GetKeycloakLoginUrl(this IConfiguration configuration)
	{
		var section = GetKeycloakSection(configuration);
		var baseUrl = configuration.GetKeycloakBaseUrl();
		var realm = configuration.GetKeycloakRealm();
		return $"{baseUrl}/realms/{realm}/protocol/openid-connect/token";
	}

	public static string GetKeycloakLogoutUrl(this IConfiguration configuration)
	{
		var section = GetKeycloakSection(configuration);
		var baseUrl = configuration.GetKeycloakBaseUrl();
		var realm = configuration.GetKeycloakRealm();
		return $"{baseUrl}/realms/{realm}/protocol/openid-connect/logout";
	}

	public static string GetAuthority(this IConfiguration configuration)
	{
		var section = GetKeycloakSection(configuration);
		var baseUrl = configuration.GetKeycloakBaseUrl();
		var realm = section["realm"];
		return $"{baseUrl}/realms/{realm}";
	}

	public static string GetKeycloakRealm(this IConfiguration configuration) => configuration.GetSection(KeycloakSectionName)["realm"]!;

	public static string GetRegisterUrl(this IConfiguration configuration) =>
		$"{configuration.GetKeycloakBaseUrl()}/admin/realms/{configuration.GetKeycloakRealm()}/users";

	public static string GetKeycloakBaseUrl(this IConfiguration configuration) => GetKeycloakSection(configuration)["auth-server-url"]!;

	public static IConfigurationSection GetKeycloakSection(this IConfiguration configuration) => configuration.GetSection(KeycloakSectionName);
}
