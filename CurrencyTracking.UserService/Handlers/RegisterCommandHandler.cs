using CurrencyTracking.Entities.DbModels;
using CurrencyTracking.UserService.Commands;
using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Utils;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;

namespace CurrencyTracking.UserService.Handlers;

public class RegisterCommandHandler(
	IUserContext userContext,
	IKeycloakUserClient userClient, 
	IConfiguration configuration,
	ILogger<RegisterCommandHandler> logger) : IRequestHandler<RegisterCommand, User>
{
	public async Task<User> Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			cancellationToken.ThrowIfCancellationRequested();
		}

		var keycloakUser = new UserRepresentation
		{
			Username = request.Name,
			Enabled = true,
			Credentials = new[]
			{
				new CredentialRepresentation
				{
					Type = "password",
					Value = request.Password,
					Temporary = false
				}
			},
		};

		var response = await userClient.CreateUserWithResponseAsync(configuration.GetKeycloakRealm(), keycloakUser, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			var message = $"User cannot be created | name={request.Name}";
			logger.LogError(message);
			throw new InvalidOperationException(message);
		}

		var guid = response.Headers.Location!.Segments.Last();
		
		var id = Guid.Parse(guid);
		var password = PasswordUtils.EncryptPassword(request.Password);

		var user = new User
		{
			Id = id,
			PasswordHash = password,
			Name = request.Name,
		};

		await userContext.Users.AddAsync(user);
		await userContext.SaveChangesAsync(cancellationToken);

		return user;
	}
}
