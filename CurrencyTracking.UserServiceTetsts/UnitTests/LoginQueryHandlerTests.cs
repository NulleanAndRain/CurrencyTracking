using CurrencyTracking.Entities.DbModels;
using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Exceptions;
using CurrencyTracking.UserService.Handlers;
using CurrencyTracking.UserService.Queries;
using CurrencyTracking.UserService.Utils;
using CurrencyTracking.UserServiceTetsts.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;

namespace CurrencyTracking.UserServiceTetsts.UnitTests;

public class LoginQueryHandlerTests
{
	[Fact]
	public async Task Handle_ValidUser_ReturnsToken()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var password = "pass";

		var user = new User
		{
			Id = Guid.NewGuid(),
			Name = "Test1",
			PasswordHash = PasswordUtils.EncryptPassword(password),
		};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		var token = "token";
		var (httpClientFactoryMock, handlerMock) = MockBuilder.GetHttpClientFactoryMock(token);

		var service = new LoginQueryHandler(
			userContext: context,
			configuration: MockBuilder.GetConfigMock(),
			httpClientFactory: httpClientFactoryMock,
			logger: MockBuilder.GetLoggerMock<LoginQueryHandler>()
		);

		// Act
		var query = new LoginQuery
		{
			Name = user.Name,
			Password = password,
		};

		var result = await service.Handle(query, CancellationToken.None);
		// Assert
		Assert.NotNull(result);
		Assert.Equal(token, result);
		handlerMock.Protected().Verify(
			"SendAsync", 
			Times.Exactly(1), 
			ItExpr.IsAny<HttpRequestMessage>(), 
			ItExpr.IsAny<CancellationToken>()
		);
	}

	[Fact]
	public async Task Handle_NoUser_ThrowsUserNotFoundException()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var password = "pass";

		var user = new User
		{
			Id = Guid.NewGuid(),
			Name = "Test1",
			PasswordHash = PasswordUtils.EncryptPassword(password),
		};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		var token = "token";
		var (httpClientFactoryMock, handlerMock) = MockBuilder.GetHttpClientFactoryMock(token);

		var service = new LoginQueryHandler(
			userContext: context,
			configuration: MockBuilder.GetConfigMock(),
			httpClientFactory: httpClientFactoryMock,
			logger: MockBuilder.GetLoggerMock<LoginQueryHandler>()
		);

		// Act
		// Assert
		var query = new LoginQuery
		{
			Name = "Test2",
			Password = password,
		};

		await Assert.ThrowsAsync<UserNotFoundException>(async () => await service.Handle(query, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_InvalidPassword_ThrowsPasswordsDoNotMatchException()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var password = "pass";

		var user = new User
		{
			Id = Guid.NewGuid(),
			Name = "Test1",
			PasswordHash = PasswordUtils.EncryptPassword(password),
		};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		var token = "token";
		var (httpClientFactoryMock, handlerMock) = MockBuilder.GetHttpClientFactoryMock(token);

		var service = new LoginQueryHandler(
			userContext: context,
			configuration: MockBuilder.GetConfigMock(),
			httpClientFactory: httpClientFactoryMock,
			logger: MockBuilder.GetLoggerMock<LoginQueryHandler>()
		);

		// Act
		// Assert
		var query = new LoginQuery
		{
			Name = user.Name,
			Password = "incorrectpassword",
		};

		await Assert.ThrowsAsync<PasswordsDoNotMatchException>(async () => await service.Handle(query, CancellationToken.None));
	}
}
