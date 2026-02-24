using CurrencyTracking.Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracking.UserService.Data;

public interface IUserContext
{
	DbSet<User> Users { get; }
}
