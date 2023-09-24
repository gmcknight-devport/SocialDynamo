using Account.Models.Users;

namespace Account.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<bool> IsUserIdUnique(string userId);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(string userId);
        Task<User> GetUserAsync(string userId);
        Task<User> GetUserByEmailAsync(string emailAddress);
        Task UpdateUserAsync(User user);
    }
}