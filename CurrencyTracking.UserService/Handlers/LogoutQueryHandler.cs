using CurrencyTracking.UserService.Exceptions;
using CurrencyTracking.UserService.Queries;
using CurrencyTracking.UserService.Utils;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CurrencyTracking.UserService.Handlers;

public class LogoutQueryHandler(
	IConfiguration configuration,
	IHttpClientFactory httpClientFactory,
	ILogger<LogoutQueryHandler> logger) : IRequestHandler<LogoutQuery>
{
	public async Task Handle(LogoutQuery request, CancellationToken cancellationToken)
	{
		var client = httpClientFactory.CreateClient(nameof(LogoutQueryHandler));

		var dict = new Dictionary<string, string>
		{
			{ "client_id", configuration.GetClientId() },
			{ "refresh_token", request.RefreshToken }
		};

		var content = new FormUrlEncodedContent(dict);
		var response = await client.PostAsync(configuration.GetKeycloakLogoutUrl(), content);

		if (!response.IsSuccessStatusCode)
		{
			logger.LogError("error while logging out user");
			throw new LogoutException();
		}
		logger.LogInformation("user successfully logged out");
	}
}
