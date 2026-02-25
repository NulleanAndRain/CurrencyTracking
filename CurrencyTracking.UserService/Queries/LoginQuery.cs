using CurrencyTracking.Entities.DbModels;
using MediatR;

namespace CurrencyTracking.UserService.Queries;

public record LoginQuery : IRequest<string>
{
	public string Name { get; init; }
	public string Password { get; init; }
}
