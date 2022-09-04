using Microsoft.AspNetCore.Mvc;

namespace Account.API.Infrastructure.Repositories
{
    public interface IAuthenticationRepository
    {
        public Task<bool> AuthenticateUser(int userId, string password);
        public Task RemoveToken(int userId);

        public Task<IActionResult?> GetRefreshToken(int userId);

        public Task UpdateToken(int userId, string refreshToken, DateTime expiresAt);
    }
}