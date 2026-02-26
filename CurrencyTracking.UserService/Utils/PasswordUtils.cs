using Konscious.Security.Cryptography;
using System.Text;

namespace CurrencyTracking.UserService.Utils;

public static class PasswordUtils
{
	public static string EncryptPassword(string password)
	{
		var bytes = Encoding.UTF8.GetBytes(password);
		var argon2 = new Argon2d(bytes);
		argon2.DegreeOfParallelism = 1;
		argon2.MemorySize = 1024;
		argon2.Iterations = 5;
		var hash = argon2.GetBytes(128)
			.Select(x => x == (byte)0 ? (byte)1 : x)
			.ToArray();

		return Encoding.UTF8.GetString(hash);
	}
}
