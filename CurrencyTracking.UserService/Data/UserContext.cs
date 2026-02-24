using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.UserService.Data;

public class UserContext : DbContext, IUserContext
{
	public DbSet<User> Users { get; init; }
}
