using CurrencyTracking.UserService.Data;
using CurrencyTracking.UserService.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.UserService.Handlers;

public class IsUserExistQueryHandler(IUserContext userContext) : IRequestHandler<IsUserExistQuery, bool>
{
	public async Task<bool> Handle(IsUserExistQuery request, CancellationToken cancellationToken)
	{
		return await userContext.Users.AnyAsync(x => x.Name == request.Name, cancellationToken);
	}
}
