using CurrencyTracking.UserService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net.Http.Json;
using System.Net;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using MediatR;
using CurrencyTracking.UserService.Queries;

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

	public static (IHttpClientFactory, Mock<HttpMessageHandler>) GetHttpClientFactoryMock(string returnValue)
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
			})
			.Verifiable();

		var httpClient = new HttpClient(handlerMock.Object);
		httpClient.BaseAddress = new Uri(TestUri);

		var factoryMock = new Mock<IHttpClientFactory>();
		factoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
			.Returns(httpClient);

		return (factoryMock.Object, handlerMock);
	}

	public static IKeycloakUserClient GetKeycloakUserClientMock(Guid userId)
	{
		var responseUri = new Uri($"{TestUri}/test/{userId}");
		var returnMessage = new HttpResponseMessage
		{
			StatusCode = HttpStatusCode.OK,
			Headers = { Location = responseUri }
		};
		var clientMock = new Mock<IKeycloakUserClient>();
		clientMock.Setup(x => x.CreateUserWithResponseAsync(It.IsAny<string>(), It.IsAny<UserRepresentation>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(returnMessage);
		return clientMock.Object;
	}

	public static IMediator GetMediatorMock(bool result)
	{
		var mediatorMock = new Mock<IMediator>();
		mediatorMock.Setup(x => x.Send(It.IsAny<IsUserExistQuery>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(result);
		return mediatorMock.Object;
	}
}
