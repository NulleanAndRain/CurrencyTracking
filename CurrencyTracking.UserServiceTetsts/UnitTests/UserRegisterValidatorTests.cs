using CurrencyTracking.UserApi.Models;
using CurrencyTracking.UserApi.Validation;
using CurrencyTracking.UserServiceTetsts.Utils;

namespace CurrencyTracking.UserServiceTetsts.UnitTests;

public class UserRegisterValidatorTests
{
	[Fact]
	public async Task Validate_CorrectNotExistingUser_ReturnsSuccess()
	{
		// Arrange
		var userExists = false;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserRegisterValidator(mediatorMock);

		var userName = "test";
		var pass = "Qw12345!";

		// Act
		var userRegisterModel = new RegisterModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(userRegisterModel);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsValid);
	}

	[Fact]
	public async Task Validate_CorrectExistingUser_ReturnsError()
	{
		// Arrange
		var userExists = true;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserRegisterValidator(mediatorMock);

		var userName = "test";
		var pass = "Qw12345!";

		// Act
		var userRegisterModel = new RegisterModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(userRegisterModel);

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
		var userExists = false;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserRegisterValidator(mediatorMock);

		var userName = "test";
		var pass = "Qw12345"; // < 8 symbols, missing special symbols

		// Act
		var userRegisterModel = new RegisterModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(userRegisterModel);

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
		var userExists = false;
		var mediatorMock = MockBuilder.GetMediatorMock(userExists);
		var validator = new UserRegisterValidator(mediatorMock);

		var userName = "q"; // < 4 symbols
		var pass = "Qw12345!";

		// Act
		var userRegisterModel = new RegisterModel
		{
			Name = userName,
			Password = pass,
		};

		var result = await validator.ValidateAsync(userRegisterModel);

		// Assert
		Assert.NotNull(result);
		Assert.False(result.IsValid);
		Assert.NotNull(result.Errors);
		Assert.NotEmpty(result.Errors);
	}
}
