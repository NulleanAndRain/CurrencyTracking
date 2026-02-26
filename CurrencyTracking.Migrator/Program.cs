using CurrencyTracking.UserApi.Migrations;
using FluentMigrator.Runner;

var builder = Host.CreateApplicationBuilder(args);

var serviceProvider = new ServiceCollection()
	.AddFluentMigratorCore()
	.ConfigureRunner(rb => rb
		.AddPostgres()
		.WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
		.ScanIn(typeof(Init).Assembly).For.Migrations()
	)
	.AddLogging(lb => lb.AddConsole())
	.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
	var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
	runner.MigrateUp();
}