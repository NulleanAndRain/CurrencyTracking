using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.CurrencyUpdateWorker.Models;
using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyTracking.CurrencyUpdateWorker.Jobs;

public class CurrencyUpdaterJob(
	ILogger<CurrencyUpdaterJob> logger,
	ICurrencyContext currencyContext,
	IHttpClientFactory httpClientFactory,
	IConfiguration configuration) : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		logger.LogInformation($"{nameof(CurrencyUpdaterJob)} started");

		var url = configuration["CbrUrl"]!;
		var httpClient = httpClientFactory.CreateClient();

		var result = await httpClient.GetAsync(url);
		var content = await result.Content.ReadAsStreamAsync();
		var serializer = new XmlSerializer(typeof(ValCurs));
		var valCurs = (ValCurs)serializer.Deserialize(content)!;

		var currenciesList = valCurs.Valute.Select(x => new Currency
		{
			Id = x.ID,
			Name = x.Name,
			Rate = x.VunitRate
		}).ToList();

		var currenciesInDb = await currencyContext.Currencies.ToListAsync();
		
		var notPresent = currenciesList.Where(x => !currenciesInDb.Any(d => x.Id == d.Id));
		await currencyContext.Currencies.AddRangeAsync(notPresent);

		foreach (var c in currenciesInDb.Where(d => currenciesList.Any(x => x.Id == d.Id)))
		{
			var newValue = currenciesList.First(x => x.Id == c.Id).Rate;
			c.Rate = newValue;
		}

		await currencyContext.SaveChangesAsync();
	}
}
