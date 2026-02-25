using CurrencyTracking.UserService.Handlers;
using CurrencyTracking.UserService.Queries;
using CurrencyTracking.UserServiceTetsts.Utils;
using Moq.Protected;
using Moq;

namespace CurrencyTracking.UserServiceTetsts.UnitTests;

public class LogoutQueryHandlerTests
{
	[Fact]
	public async Task Handle_ValidUser_CallsApiMethod()
	{
		// Arrange
		var token = "token";
		var (httpClientFactoryMock, handlerMock) = MockBuilder.GetHttpClientFactoryMock(token);

		var service = new LogoutQueryHandler(
			configuration: MockBuilder.GetConfigMock(),
			httpClientFactory: httpClientFactoryMock,
			logger: MockBuilder.GetLoggerMock<LogoutQueryHandler>()
		);

		// Act
		var query = new LogoutQuery
		{
			RefreshToken = token,
		};

		await service.Handle(query, CancellationToken.None);

		// Assert
		handlerMock.Protected().Verify(
			"SendAsync",
			Times.Exactly(1),
			ItExpr.IsAny<HttpRequestMessage>(),
			ItExpr.IsAny<CancellationToken>()
		);
	}
}
