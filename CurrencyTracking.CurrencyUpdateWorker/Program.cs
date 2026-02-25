using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.CurrencyUpdateWorker.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ICurrencyContext, CurrencyContext>();
builder.Services.AddDbContext<CurrencyContext>(o =>
{
	o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
		x => x.MigrationsAssembly("CurrencyTracking.Migrations"));
});

builder.Services.AddQuartz(cfg =>
{
	var jobKey = new JobKey(nameof(CurrencyUpdaterJob));
	cfg.AddJob<CurrencyUpdaterJob>(jobKey, configure: x => { });

	cfg.AddTrigger(trigger =>
	{
		trigger.ForJob(jobKey)
			.StartNow()
			.WithCronSchedule("0 2 * * *");
	});
});

builder.Services.AddQuartzHostedService(o =>
{
	o.WaitForJobsToComplete = true;
	o.AwaitApplicationStarted = true;
});

var host = builder.Build();
host.Run();
