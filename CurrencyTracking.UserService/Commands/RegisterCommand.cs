using CurrencyTracking.Entities.DbModels;
using MediatR;

namespace CurrencyTracking.UserService.Commands;

public record RegisterCommand : IRequest<User>
{
	public string Name { get; set; }
	public string Password { get; set; }
}
