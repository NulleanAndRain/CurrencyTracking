using CurrencyTracking.CurrencyService.Data;
using CurrencyTracking.CurrencyService.Queries;
using CurrencyTracking.Entities.DbModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.CurrencyService.Handlers;

public class GetCurrenciesQueryHandler(ICurrencyContext currencyContext) : IRequestHandler<GetCurrenciesQuery, IEnumerable<Currency>>
{
	public async Task<IEnumerable<Currency>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			cancellationToken.ThrowIfCancellationRequested();
		}

		var result = await currencyContext.UserFavorites
			.AsNoTracking()
			.Where(x => x.UserId == request.UserId)
			.Select(x => x.Currency)
			.ToListAsync(cancellationToken);

		return result;
	}
}
