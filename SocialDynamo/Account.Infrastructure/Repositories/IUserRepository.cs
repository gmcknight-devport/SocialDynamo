using Account.Models.Users;

namespace Account.API.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task DeleteUserAsync(string userId);
        Task<User> GetUserAsync(string userId);
        Task UpdateUserAsync(User user);
    }
}