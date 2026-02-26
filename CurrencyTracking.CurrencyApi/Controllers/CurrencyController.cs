using CurrencyTracking.CurrencyService.Queries;
using CurrencyTracking.Entities.DbModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CurrencyTracking.CurrencyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(ILogger<CurrencyController> logger, IMediator mediator) : Controller
{
	[Authorize]
	[HttpGet("favorites")]
	public async Task<Results<Ok<CurrencyFavoritesResponse>, UnauthorizedHttpResult>> Get(CancellationToken cancellationToken)
	{
		logger.LogInformation("list tracked currencuies");

		var userGuidString = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userGuidString == null)
		{
			return TypedResults.Unauthorized();
		}

		var userId = Guid.Parse(userGuidString);
		var query = new GetCurrenciesQuery { UserId = userId };
		var result = await mediator.Send(query, cancellationToken);

		return TypedResults.Ok(new CurrencyFavoritesResponse {
			Data = result
		});
	}
}
