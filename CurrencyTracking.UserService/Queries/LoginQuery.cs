using CurrencyTracking.UserService.Models;
using MediatR;

namespace CurrencyTracking.UserService.Queries;

public record LoginQuery : IRequest<JwtModel>
{
	public string Name { get; init; }
	public string Password { get; init; }
}
