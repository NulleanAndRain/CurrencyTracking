using CurrencyTracking.UserApi.Models;
using CurrencyTracking.UserApi.Validation;
using CurrencyTracking.UserServiceTetsts.Utils;

namespace CurrencyTracking.UserServiceTetsts.UnitTests;

public class UserLoginValidatorTests
{
	[Fact]
	public async Task Validate_CorrectExistingUser_ReturnsSuccess()
	{
		// Arrange
		var userExists = true;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserLoginValidator(mediatorMock);

		var userName = "test";
		var pass = "Qw12345!";

		// Act
		var loginModel = new LoginModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(loginModel);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsValid);
	}

	[Fact]
	public async Task Validate_NotExistingUser_ReturnsError()
	{
		// Arrange
		var userExists = false;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserLoginValidator(mediatorMock);

		var userName = "test";
		var pass = "Qw12345!";

		// Act
		var loginModel = new LoginModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(loginModel);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsValid);
		Assert.NotNull(result.Errors);
		Assert.NotEmpty(result.Errors);
	}

	[Fact]
	public async Task Validate_IncorrectPasswordUser_ReturnsError()
	{
		// Arrange
		var userExists = true;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserLoginValidator(mediatorMock);

		var userName = "test";
		var pass = "Qw12345"; // < 8 symbols, missing special symbols

		// Act
		var loginModel = new LoginModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(loginModel);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsValid);
		Assert.NotNull(result.Errors);
		Assert.NotEmpty(result.Errors);
	}

	[Fact]
	public async Task Validate_IncorrectUsernameUser_ReturnsError()
	{
		// Arrange
		var userExists = true;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserLoginValidator(mediatorMock);

		var userName = "q"; // < 4 symbols
		var pass = "Qw12345!";

		// Act
		var loginModel = new LoginModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(loginModel);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsValid);
		Assert.NotNull(result.Errors);
		Assert.NotEmpty(result.Errors);
	}
}
