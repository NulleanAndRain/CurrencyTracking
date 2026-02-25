using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Handlers;
using CurrencyTracking.UserServiceTetsts.Utils;
using Microsoft.EntityFrameworkCore;
using CurrencyTracking.UserService.Commands;
using CurrencyTracking.Entities.DbModels;

namespace CurrencyTracking.UserServiceTetsts.UnitTests;

public class RegisterCommandHandlerTests
{
	[Fact]
	public async Task Handle_ValidUser_ReturnsToken()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var userName = "user1";
		var password = "pass";

		var userId = Guid.NewGuid();

		var keycloakUserClientMock = MockBuilder.GetKeycloakUserClientMock(userId);

		var service = new RegisterCommandHandler(
			userContext: context,
			configuration: MockBuilder.GetConfigMock(),
			userClient: keycloakUserClientMock,
			logger: MockBuilder.GetLoggerMock<RegisterCommandHandler>()
		);

		// Act
		var query = new RegisterCommand
		{
			Name = userName,
			Password = password,
		};

		var result = await service.Handle(query, CancellationToken.None);
		// Assert
		Assert.NotNull(result);

		var user = context.Users.FirstOrDefault(x => x.Id == userId);
		Assert.NotNull(user);
		Assert.Equal(userName, result.Name);
		Assert.Equal(userName, user.Name);
	}

	[Fact]
	public async Task Handle_UserExists_ThrowsInvalidOperationException()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var userId = Guid.NewGuid();
		var userName = "user1";
		var password = "pass";

		var user = new User
		{
			Id = userId,
			Name = userName,
			PasswordHash = password,
		};
		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		var keycloakUserClientMock = MockBuilder.GetKeycloakUserClientMock(userId);

		var service = new RegisterCommandHandler(
			userContext: context,
			configuration: MockBuilder.GetConfigMock(),
			userClient: keycloakUserClientMock,
			logger: MockBuilder.GetLoggerMock<RegisterCommandHandler>()
		);

		// Act
		// Assert
		var query = new RegisterCommand
		{
			Name = userName,
			Password = password,
		};

		await Assert.ThrowsAsync<InvalidOperationException>(async () => await service.Handle(query, CancellationToken.None));
	}
}
