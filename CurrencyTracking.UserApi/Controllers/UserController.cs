using CurrencyTracking.UserApi.Models;
using CurrencyTracking.UserApi.Validation;
using CurrencyTracking.UserService.Commands;
using CurrencyTracking.UserService.Exceptions;
using CurrencyTracking.UserService.Models;
using CurrencyTracking.UserService.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CurrencyTracking.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UserController(ILogger<UserController> logger, IMediator mediator) : Controller
{
	[HttpPost("register")]
	public async Task<Results<Ok<JwtModel>, BadRequest<ProblemDetails>>> Register([FromBody] RegisterModel model, CancellationToken cancellationToken)
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
			var msg = $"Произошла ошибка во время регистрации | {nameof(PasswordsDoNotMatchException)}";
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(GetLoginProblem(ex));
		}
		catch (LoginException ex)
		{
			var msg = $"Произошла ошибка во время регистрации | {nameof(LoginException)}";
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(GetLoginProblem(ex));
		}
		catch (UserNotFoundException ex)
		{
			var msg = "Произошла ошибка во время регистрации";
			var problem = new ProblemDetails
			{
				Type = ex.GetType().Name,
				Status = (int)HttpStatusCode.BadRequest,
				Title = msg,
				Detail = msg,
				Instance = HttpContext.Request.Path
			};
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(problem);
		}
		catch (Exception ex)
		{
			var msg = "Произошла неизвестная ошибка во время регистрации";
			var problem = new ProblemDetails
			{
				Type = "UntypedRegistrationException",
				Status = (int)HttpStatusCode.BadRequest,
				Title = msg,
				Detail = msg,
				Instance = HttpContext.Request.Path
			};
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(problem);
		}
	}

	[HttpPost("login")]
	public async Task<Results<Ok<JwtModel>, BadRequest<ProblemDetails>>> Login([FromBody] LoginModel model, CancellationToken cancellationToken)
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
			var token = await mediator.Send(query, cancellationToken);
			return TypedResults.Ok(token);
		}
		catch (PasswordsDoNotMatchException ex)
		{
			var msg = $"Произошла ошибка во время входа | {nameof(PasswordsDoNotMatchException)}";
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(GetLoginProblem(ex, "Неверный пароль"));
		}
		catch (LoginException ex)
		{
			var msg = $"Произошла ошибка во время входа | {nameof(PasswordsDoNotMatchException)}";
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(GetLoginProblem(ex));
		}
		catch (UserNotFoundException ex)
		{
			var msg = $"Произошла ошибка во время входа | {nameof(PasswordsDoNotMatchException)}";
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(GetLoginProblem(ex, "Пользователь не найден"));
		}
		catch (Exception ex)
		{
			var msg = "Произошла неизвестная ошибка во время входа";
			var problem = new ProblemDetails
			{
				Type = "UntypedLoginException",
				Status = (int)HttpStatusCode.BadRequest,
				Title = msg,
				Detail = msg,
				Instance = HttpContext.Request.Path
			};
			logger.LogError(ex, msg);
			return TypedResults.BadRequest(problem);
		}
	}

	[HttpPost("logout")]
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
		catch (Exception ex)
		{
			var msg = "Произошла ошибка во время выхода";
			var problem = new ProblemDetails
			{
				Type = nameof(LogoutException),
				Status = (int)HttpStatusCode.BadRequest,
				Title = msg,
				Detail = msg,
				Instance = HttpContext.Request.Path
			};
			logger.LogError(ex, msg);
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
