using CurrencyTracking.Entities.DbModels;
using CurrencyTracking.UserService.Commands;
using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Services;
using CurrencyTracking.UserService.Utils;
using Keycloak.AuthServices.Sdk.Admin.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurrencyTracking.UserService.Handlers;

public class RegisterCommandHandler(
	IUserContext userContext,
	IHttpClientFactory httpClientFactory,
	IKeycloakTokenService tokenService,
	IConfiguration configuration,
	ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, User>
{
	public async Task<User> Handle(RegisterCommand command, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			cancellationToken.ThrowIfCancellationRequested();
		}

		var keycloakUser = new UserRepresentation
		{
			Username = command.Name,
			Enabled = true,
			Email = $"{command.Name}@{configuration.GetKeycloakRealm()}",
			EmailVerified = true,
			Credentials = new[]
			{
				new CredentialRepresentation
				{
					Type = "password",
					Value = command.Password,
					Temporary = false
				}
			},
		};

		var token = await tokenService.GetKeycloakTokenAsync();
		var url = configuration.GetRegisterUrl();
		var httpClient = httpClientFactory.CreateClient();

		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await httpClient.PostAsJsonAsync(url, keycloakUser, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			var message = $"User cannot be created | name={command.Name}";
			logger.LogError(message);
			throw new InvalidOperationException(message);
		}

		var guid = response.Headers.Location!.Segments.Last();
		var id = Guid.Parse(guid);


		var password = PasswordUtils.EncryptPassword(command.Password);

		var user = new User
		{
			Id = id,
			PasswordHash = password,
			Name = command.Name,
		};

		logger.LogInformation($"Registered user in keycloak, received user id: {id} | user name: {user.Name}");

		await userContext.Users.AddAsync(user);
		await userContext.SaveChangesAsync(cancellationToken);

		return user;
	}
}
