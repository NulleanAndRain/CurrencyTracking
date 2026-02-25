using MediatR;

namespace CurrencyTracking.UserService.Queries;

public record IsUserExistQuery : IRequest<bool>
{
	public string Name { get; init; }
}
