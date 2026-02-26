using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.CurrencyUpdateWorker.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Text;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ICurrencyContext, CurrencyContext>();
builder.Services.AddDbContext<CurrencyContext>(o =>
{
	o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var jobKey = new JobKey(nameof(CurrencyUpdaterJob));

builder.Services.AddQuartz(cfg =>
{
	cfg.AddJob<CurrencyUpdaterJob>(jobKey, configure: x => { });

	cfg.AddTrigger(trigger =>
	{
		trigger.ForJob(jobKey)
			.StartAt(DateTimeOffset.Now.AddDays(-1))
			.WithCronSchedule(builder.Configuration["CronSchedule"]!);
	});
});
builder.Services.AddHttpClient();

builder.Services.AddQuartzHostedService(o =>
{
	o.WaitForJobsToComplete = true;
	o.AwaitApplicationStarted = true;
});

var host = builder.Build();
host.Run();
