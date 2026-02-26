
using CurrencyTracking.UserService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CurrencyTracking.UserService.Services;

public class KeycloakTokenService(
	IHttpClientFactory httpClientFactory,
	IConfiguration configuration,
	ILogger<KeycloakTokenService> logger) : IKeycloakTokenService
{
	private JsonWebToken? _token;
	private SemaphoreSlim _sync = new SemaphoreSlim(1, 1);
	private const int RequestWaitTimeout = 1;

	public async Task<string> GetKeycloakTokenAsync()
	{
		if (_token == null || _token.ValidTo < DateTime.UtcNow)
		{
			logger.LogInformation("Token is expired or null");
			if (await _sync.WaitAsync(RequestWaitTimeout))
			{
				try
				{
					logger.LogInformation("Requesting new token");
					var client = httpClientFactory.CreateClient();

					var dict = new Dictionary<string, string>
					{
						{ "client_id", GetAdminId() },
						{ "client_secret", GetSecret() },
						{ "grant_type", "client_credentials" },
					};
					var content = new FormUrlEncodedContent(dict);
					content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
					var res = await client.PostAsync(GetAuthUrl(), content);
					var responseBody = await res.Content.ReadAsStringAsync();

					logger.LogInformation("Token received, parsing | {0}", responseBody);

					var tokenString = JsonSerializer.Deserialize<TokenWrapper>(responseBody)!.Token;
					_token = new JsonWebTokenHandler().ReadToken(tokenString) as JsonWebToken;

					logger.LogInformation("Token parsed");
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error occured while getting token");
				}
				finally
				{
					_sync.Release();
				}
			}
		}

		logger.LogInformation("Waiting for token");
		string ret;
		await _sync.WaitAsync();
		try
		{
			ret = _token!.EncodedToken;
		}
		finally
		{
			_sync.Release();
		}

		logger.LogInformation("Returning token");
		return ret;
	}

	private string GetAuthUrl() =>
		$"{configuration.GetKeycloakBaseUrl()}/realms/{configuration.GetKeycloakRealm()}/protocol/openid-connect/token";

	private string GetAdminId() =>
		configuration.GetKeycloakSection().GetSection("AdminClient")["username"]!;

	private string GetSecret() =>
		configuration.GetKeycloakSection().GetSection("AdminClient")["secret"]!;

	private class TokenWrapper
	{
		[JsonPropertyName("access_token")]
		public string Token { get; set; }
	}
}
