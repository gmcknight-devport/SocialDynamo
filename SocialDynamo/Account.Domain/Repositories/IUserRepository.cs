using Common.Models.Users;

namespace Common.Domain.Repositories
{
    public interface IUserRepository
    {
        bool IsUserIdUnique(string userId);
        bool IsEmailUnique(string emailAddress);
        Task AddUserAsync(User user);
        Task DeleteUserAsync(string userId);
        Task<User> GetUserAsync(string userId);
        Task<User> GetUserByEmailAsync(string emailAddress);
        Task UpdateUserAsync(User user);
    }
}