using Account.Models.Users;

namespace Account.API.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int userId);
        Task<User> GetUserAsync(int userId);
        Task<User> GetUserAsync(string emailAddress);
        Task UpdateUserAsync(User user);
    }
}