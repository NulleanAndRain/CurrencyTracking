using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Exceptions;
using CurrencyTracking.UserService.Queries;
using CurrencyTracking.UserService.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CurrencyTracking.UserService.Handlers;

public class LoginQueryHandler(
	IUserContext userContext,
	IConfiguration configuration,
	IHttpClientFactory httpClientFactory,
	ILogger<LoginQueryHandler> logger) : IRequestHandler<LoginQuery, string>
{
	public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
	{
		var user = userContext.Users.AsNoTracking().FirstOrDefault(x => x.Name == request.Name);
		if (user == null)
		{
			logger.LogWarning("user {0} not found while attempting to authorize", request.Name);
			throw new UserNotFoundException();
		}

		if (PasswordUtils.EncryptPassword(request.Password) != user.PasswordHash)
		{
			logger.LogWarning("unsuccessfull attempt to authorize user {0}", request.Name);
			throw new PasswordsDoNotMatchException();
		}

		var httpClient = httpClientFactory.CreateClient(nameof(LoginQueryHandler));
		
		var dict = new Dictionary<string, string>
		{
			{ "grant_type", "password" },
			{ "client_id", "my-backend-client" },
            { "username", request.Name },
			{ "password", request.Password },
			{ "scope", "openid" }
		};
		var content = new FormUrlEncodedContent(dict);
		var response = await httpClient.PostAsync(configuration.GetKeycloakLoginUrl(), content);


		if (!response.IsSuccessStatusCode)
		{
			throw new LoginException();
		}

		return await response.Content.ReadAsStringAsync();
	}
}
