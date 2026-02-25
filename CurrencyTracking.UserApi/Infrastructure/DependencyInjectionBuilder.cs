using CurrencyTracking.UserService.Data;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CurrencyTracking.UserApi.Infrastructure;

public static class DependencyInjectionBuilder
{
	public static IServiceCollection AddServices(this WebApplicationBuilder builder)
	{
		var services = builder.Services;

		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddScoped<IUserContext, UserContext>();
		services.AddDbContext<UserContext>(o =>
		{
			o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
				x => x.MigrationsAssembly("CurrencyTracking.Migrations"));
		});

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});

		services.AddKeycloakWebApiAuthentication(builder.Configuration);
		services.AddKeycloakAdminHttpClient(builder.Configuration);

		return services;
	}
}
