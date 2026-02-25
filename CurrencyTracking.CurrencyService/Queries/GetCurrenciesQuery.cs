using CurrencyTracking.Entities.DbModels;
using MediatR;

namespace CurrencyTracking.CurrencyService.Queries;

public record GetCurrenciesQuery : IRequest<IEnumerable<Currency>>
{
	public Guid UserId { get; set; }
}
