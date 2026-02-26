using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Services;
using CurrencyTracking.UserService.Utils;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.UserApi.Infrastructure;

public static class DependencyInjectionBuilder
{
	public static IServiceCollection AddServices(this WebApplicationBuilder builder)
	{
		var services = builder.Services;

		services.AddHttpClient();
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddLogging(cfg =>
		{
			cfg.AddConsole();
			cfg.AddDebug();
			cfg.SetMinimumLevel(LogLevel.Debug);
		});

		services.AddScoped<IUserContext, UserContext>();
		services.AddDbContext<UserContext>(o =>
		{
			o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
				x => x.MigrationsAssembly("CurrencyTracking.Migrations"));
		});

		services.AddSingleton<IKeycloakTokenService, KeycloakTokenService>();

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
		});

		services.AddKeycloakWebApiAuthentication(builder.Configuration, cfg =>
		{
			cfg.Authority = builder.Configuration.GetAuthority();
			cfg.Audience = builder.Configuration.GetClientId();
			cfg.RequireHttpsMetadata = false;
			cfg.MetadataAddress = $"{builder.Configuration.GetAuthority()}/.well-known/openid-configuration";
		});
		services.AddAuthorization()
			.AddCookiePolicy(o =>
			{
				o.MinimumSameSitePolicy = SameSiteMode.None;
			})
			.AddKeycloakAuthorization(builder.Configuration)
			.AddAuthorizationServer(builder.Configuration);

		services.AddKeycloakAdminHttpClient(builder.Configuration);

		return services;
	}
}
