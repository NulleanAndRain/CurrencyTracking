using MediatR;

namespace CurrencyTracking.UserService.Queries;

public record LogoutQuery : IRequest
{
	public string RefreshToken { get; init; }
}
