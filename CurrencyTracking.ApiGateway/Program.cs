using Keycloak.AuthServices.Authentication;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelot();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, cfg =>
{
	cfg.RequireHttpsMetadata = false;
});
// Add services to the container.

var app = builder.Build();

app.UseRouting();

app.UseOcelot().Wait();

app.Run();
