using CurrencyTracking.UserService.Queries;
using CurrencyTracking.UserService.Services;
using CurrencyTracking.UserService.Utils;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace CurrencyTracking.UserServiceTetsts.Utils;

public static class MockBuilder
{
	private const string TestUri = "https://test.net";

	public static IConfiguration GetConfigMock()
	{
		var configSectionMock = new Mock<IConfigurationSection>();
		configSectionMock.SetupGet(x => x["auth-server-url"]).Returns(TestUri);
		configSectionMock.SetupGet(x => x["realm"]).Returns("realm");
		configSectionMock.SetupGet(x => x["resource"]).Returns("resource");

		var configMock = new Mock<IConfiguration>();
		configMock.Setup(x => x.GetSection(KeycloakConfigUtils.KeycloakSectionName))
			.Returns(configSectionMock.Object);

		return configMock.Object;
	}

	public static ILogger<T> GetLoggerMock<T>()
	{
		var loggerMock = new Mock<ILogger<T>>();
		return loggerMock.Object;
	}

	public static (IHttpClientFactory, Mock<HttpMessageHandler>) GetHttpClientFactoryMock(string returnValue, string locationHeaderEnd = "")
	{
		var handlerMock = new Mock<HttpMessageHandler>();
		handlerMock
			.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(returnValue),
				Headers =
				{
					Location = new Uri($"{TestUri}/test/{locationHeaderEnd}")
				}
			})
			.Verifiable();

		var httpClient = new HttpClient(handlerMock.Object);
		httpClient.BaseAddress = new Uri(TestUri);

		var factoryMock = new Mock<IHttpClientFactory>();
		factoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
			.Returns(httpClient);

		return (factoryMock.Object, handlerMock);
	}

	public static IMediator GetMediatorMock(bool result)
	{
		var mediatorMock = new Mock<IMediator>();
		mediatorMock.Setup(x => x.Send(It.IsAny<IsUserExistQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(result);
		return mediatorMock.Object;
	}

	public static IKeycloakTokenService GetTokenServiceMock(string token)
	{
		var tokenServiceMock = new Mock<IKeycloakTokenService>();
		tokenServiceMock.Setup(x => x.GetKeycloakTokenAsync())
			.ReturnsAsync(token);

		return tokenServiceMock.Object;
	}
}
