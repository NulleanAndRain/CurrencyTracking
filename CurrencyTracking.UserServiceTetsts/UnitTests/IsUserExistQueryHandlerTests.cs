using CurrencyTracking.Entities.DbModels;
using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Handlers;
using CurrencyTracking.UserService.Queries;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.UserServiceTetsts.UnitTests;

public class IsUserExistQueryHandlerTests
{
	[Fact]
	public async Task Handle_UserExists_ReturnsTrue()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var user = new User
		{
			Id = Guid.NewGuid(),
			Name = "Test1",
			PasswordHash = "pass"
		};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		var service = new IsUserExistQueryHandler(context);

		// Act
		var query = new IsUserExistQuery
		{
			Name = user.Name,
		};
		var result = await service.Handle(query, CancellationToken.None);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task Handle_UserNotExists_ReturnsFalse()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<UserContext>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new UserContext(contextOptions);

		var user = new User
		{
			Id = Guid.NewGuid(),
			Name = "Test1",
			PasswordHash = "pass"
		};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		var service = new IsUserExistQueryHandler(context);

		// Act
		var query = new IsUserExistQuery
		{
			Name = "Test2",
		};
		var result = await service.Handle(query, CancellationToken.None);

		// Assert
		Assert.False(result);
	}
}