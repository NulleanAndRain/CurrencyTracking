using CurrencyTracking.UserApi.Models;
using CurrencyTracking.UserApi.Validation;
using CurrencyTracking.UserService.Commands;
using CurrencyTracking.UserService.Exceptions;
using CurrencyTracking.UserService.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CurrencyTracking.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ILogger<UserController> logger, IMediator mediator) : Controller
{
	[HttpPost("register")]
	public async Task<Results<Ok<string>, BadRequest<ProblemDetails>>> Register([FromBody] RegisterModel model, CancellationToken cancellationToken)
	{
		var validator = new UserRegisterValidator(mediator);
		var validationResult = await validator.ValidateAsync(model, cancellationToken);

		if (!validationResult.IsValid)
		{
			var problemDetails = new ProblemDetails
			{
				Title = "Произошла одна или несколько ошибок при валидации",
				Status = StatusCodes.Status400BadRequest,
				Instance = HttpContext.Request.Path,
				Extensions = validationResult.Errors
					.GroupBy(x => x.PropertyName)
					.ToDictionary(
						g => g.Key,
						g => g.Select(x => x.ErrorMessage).ToArray() as object
					) as IDictionary<string, object?>
			};

			return TypedResults.BadRequest(problemDetails);
		}
		try
		{
			logger.LogInformation("user registration");
			var command = new RegisterCommand
			{
				Name = model.Name,
				Password = model.Password,
			};
			var user = await mediator.Send(command, cancellationToken);
			logger.LogInformation("user {0} registered, logging in", model.Name);
			var loginQuery = new LoginQuery
			{
				Name = user.Name,
				Password = model.Password,
			};
			var token = await mediator.Send(loginQuery, cancellationToken);
			return TypedResults.Ok(token);
		}
		catch (PasswordsDoNotMatchException ex)
		{
			return TypedResults.BadRequest(GetLoginProblem(ex));
		}
		catch (LoginException ex)
		{
			return TypedResults.BadRequest(GetLoginProblem(ex));
		}
		catch (UserNotFoundException ex)
		{
			var problem = new ProblemDetails
			{
				Type = ex.GetType().Name,
				Status = (int)HttpStatusCode.BadRequest,
				Title = "Произошла ошибка во время регистрации",
				Detail = "Произошла ошибка во время регистрации",
				Instance = HttpContext.Request.Path
			};
			return TypedResults.BadRequest(problem);
		}
		catch
		{
			var problem = new ProblemDetails
			{
				Type = "UntypedRegistrationException",
				Status = (int)HttpStatusCode.BadRequest,
				Title = "Произошла неизвестная ошибка во время регистрации",
				Detail = "Произошла неизвестная ошибка во время регистрации",
				Instance = HttpContext.Request.Path
			};
			return TypedResults.BadRequest(problem);
		}
	}

	[HttpPost("login")]
	public async Task<Results<Ok<string>, BadRequest<ProblemDetails>>> Login([FromBody] LoginModel model, CancellationToken cancellationToken)
	{
		var validator = new UserLoginValidator(mediator);
		var validationResult = await validator.ValidateAsync(model, cancellationToken);

		if (!validationResult.IsValid)
		{
			var problemDetails = new ProblemDetails
			{
				Title = "Произошла одна или несколько ошибок при валидации",
				Status = StatusCodes.Status400BadRequest,
				Instance = HttpContext.Request.Path,
				Extensions = validationResult.Errors
					.GroupBy(x => x.PropertyName)
					.ToDictionary(
						g => g.Key,
						g => g.Select(x => x.ErrorMessage).ToArray() as object
					) as IDictionary<string, object?>
			};

			return TypedResults.BadRequest(problemDetails);
		}
		try
		{
			var query = new LoginQuery
			{
				Name = model.Name,
				Password = model.Password,
			};
			var res = await mediator.Send(query, cancellationToken);
			return TypedResults.Ok(res);
		}
		catch (PasswordsDoNotMatchException ex)
		{
			return TypedResults.BadRequest(GetLoginProblem(ex, "Неверный пароль"));
		}
		catch (LoginException ex)
		{
			return TypedResults.BadRequest(GetLoginProblem(ex));
		}
		catch (UserNotFoundException ex)
		{
			return TypedResults.BadRequest(GetLoginProblem(ex, "Пользователь не найден"));
		}
		catch
		{
			var problem = new ProblemDetails
			{
				Type = "UntypedLoginException",
				Status = (int)HttpStatusCode.BadRequest,
				Title = "Произошла неизвестная ошибка во время входа",
				Detail = "Произошла неизвестная ошибка во время входа",
				Instance = HttpContext.Request.Path
			};
			return TypedResults.BadRequest(problem);
		}
	}

	[HttpPost("logout")]
	[Authorize]
	public async Task<Results<Ok, BadRequest<ProblemDetails>>> Logout([FromBody] LogoutModel model, CancellationToken cancellationToken)
	{
		try
		{
			var query = new LogoutQuery
			{
				RefreshToken = model.RefreshToken,
			};
			await mediator.Send(query, cancellationToken);

			return TypedResults.Ok();
		}
		catch (Exception)
		{
			var problem = new ProblemDetails
			{
				Type = nameof(LogoutException),
				Status = (int)HttpStatusCode.BadRequest,
				Title = "Произошла ошибка во время выхода",
				Detail = "Произошла ошибка во время выхода",
				Instance = HttpContext.Request.Path
			};
			return TypedResults.BadRequest(problem);
		}
	}

	private ProblemDetails GetLoginProblem(Exception ex, string detail = "Произошла ошибка во время входа")
	{
		return new ProblemDetails
		{
			Type = ex.GetType().Name,
			Status = (int)HttpStatusCode.BadRequest,
			Title = "Произошла ошибка во время входа",
			Detail = detail,
			Instance = HttpContext.Request.Path
		};
	}
}
