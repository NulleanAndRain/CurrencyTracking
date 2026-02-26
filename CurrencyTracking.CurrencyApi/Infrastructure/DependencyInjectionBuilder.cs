using CurrencyTracking.CurrencyService.Data;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyApi.Infrastructure;

public static class DependencyInjectionBuilder
{
	public const string KeycloakSectionName = "Keycloak";

	public static IServiceCollection AddServices(this WebApplicationBuilder builder)
	{
		var services = builder.Services;

		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddLogging(cfg =>
		{
			cfg.AddConsole();
			cfg.AddDebug();
			cfg.SetMinimumLevel(LogLevel.Debug);
		});

		services.AddScoped<ICurrencyContext, CurrencyContext>();
		services.AddDbContext<CurrencyContext>(o =>
		{
			o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
		});

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
		});

		services.AddKeycloakWebApiAuthentication(builder.Configuration, cfg =>
		{
			cfg.RequireHttpsMetadata = false;
		});
		services.AddAuthorization()
			.AddKeycloakAuthorization(builder.Configuration)
			.AddAuthorizationServer(builder.Configuration);

		return services;
	}
}
