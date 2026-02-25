using System.Text.RegularExpressions;

namespace CurrencyTracking.UserApi.Validation;

public static class ValidationUtils
{
	public static readonly Regex PasswordRegex = new Regex(@"^(?=.*[a-zа-я])(?=.*[A-ZА-Я])(?=.*\d)(?=.*[@$!%*?&\-\.])[A-Za-zА-Яа-я\d@$!%*?&\-\.]{8,255}$");
}
