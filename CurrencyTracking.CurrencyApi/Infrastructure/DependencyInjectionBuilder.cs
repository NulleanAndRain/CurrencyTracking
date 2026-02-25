using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.CurrencyService.Handlers;
using Keycloak.AuthServices.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CurrencyTracking.CurrencyApi.Infrastructure;

public static class DependencyInjectionBuilder
{
	public static IServiceCollection AddServices(this WebApplicationBuilder builder)
	{
		var services = builder.Services;

		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddScoped<ICurrencyContext, CurrencyContext>();
		services.AddDbContext<CurrencyContext>(o =>
		{
			o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
				x => x.MigrationsAssembly("CurrencyTracking.Migrations"));
		});

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
			cfg.RegisterServicesFromAssembly(typeof(GetCurrenciesQueryHandler).Assembly);
		});

		services.AddKeycloakWebApiAuthentication(builder.Configuration);

		return services;
	}
}
