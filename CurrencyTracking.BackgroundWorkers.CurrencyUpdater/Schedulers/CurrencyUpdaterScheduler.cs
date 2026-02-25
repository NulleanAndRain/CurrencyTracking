using CurrencyTracking.BackgroundWorkers.CurrencyUpdater.Jobs;
using Quartz;
using Quartz.Impl;

namespace CurrencyTracking.BackgroundWorkers.CurrencyUpdater.Schedulers;

public static class CurrencyUpdaterScheduler
{
	public static async Task Start()
	{
		var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
		await scheduler.Start();

		var job = JobBuilder.Create<CurrencyUpdaterJob>().Build();

		var trigger = TriggerBuilder.Create()
			.WithIdentity(nameof(CurrencyUpdaterJob))
			.StartNow()
			.WithCronSchedule("0 2 * * *")
			.Build();

		await scheduler.ScheduleJob(job, trigger);
	}
}
