using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.CurrencyService.Handlers;
using CurrencyTracking.CurrencyService.Queries;
using CurrencyTracking.CurrencyServiceTests.Mocks;
using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyServiceTests.UnitTests;

public class GetCurrenciesQueryHandlerTests
{
	[Fact]
	public async Task Handle_UserExists_ReturnsFavorites()
	{
		// Arrange
		var contextOptions = new DbContextOptionsBuilder<CurrencyContextMock>()
			.UseInMemoryDatabase($"InMemDb{Guid.NewGuid()}")
			.Options;
		var context = new CurrencyContextMock(contextOptions);

		var currency = new Currency
		{
			Id = "currency1",
			Name = "currency1",
			Rate = 1,
		};

		var userId = Guid.NewGuid();
		var userFavorite = new UserFavorite
		{
			UserId = userId,
			Currency = currency,
			CurrencyId = currency.Id,
		};

		await context.Currencies.AddAsync(currency);
		await context.UserFavorites.AddAsync(userFavorite);
		await context.SaveChangesAsync();

		var handler = new GetCurrenciesQueryHandler(context);

		// Act
		var request = new GetCurrenciesQuery
		{
			UserId = userId
		};

		var result = await handler.Handle(request, CancellationToken.None);
		
		// Assert
		var expectedId = currency.Id;
		Assert.NotNull(result);
		Assert.Contains(result, actual => actual.Id == expectedId);
	}
}