using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.UserService.Data;

public class UserContext : DbContext, IUserContext, IDisposable
{
	public DbSet<User> Users { get; init; }

	public UserContext(DbContextOptions options) : base(options)
	{
		Database.EnsureCreated();
	}
}
