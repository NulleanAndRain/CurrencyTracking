using CurrencyTracking.UserApi.Models;
using CurrencyTracking.UserService.Queries;
using FluentValidation;
using MediatR;

namespace CurrencyTracking.UserApi.Validation
{
	public class UserLoginValidator : AbstractValidator<LoginModel>
	{
		private IMediator Mediator { get; init; }

		public UserLoginValidator(IMediator mediator)
		{
			this.Mediator = mediator;

			RuleFor(x => x.Name)
				.NotNull()
				.Length(4, 255)
				.CustomAsync(ValidateUniqueName);

			RuleFor(x => x.Password)
				.NotNull()
				.Matches(ValidationUtils.PasswordRegex)
				.WithMessage("Пароль должен быть длиной от 8 до 255 символов и содержать строчные буквы, заглавные буквы, цифры и специальные символы (@$!%*?&-.)");
		}

		private async Task ValidateUniqueName(string name, ValidationContext<LoginModel> ctx, CancellationToken cancellationToken)
		{
			var query = new IsUserExistQuery
			{
				Name = name
			};
			var isUserExist = await Mediator.Send(query, cancellationToken);
			if (!isUserExist)
			{
				ctx.AddFailure(nameof(RegisterModel.Name), "Пользователь не найден");
			}
		}
	}
}
