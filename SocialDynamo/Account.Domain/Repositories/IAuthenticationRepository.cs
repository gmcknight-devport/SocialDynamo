using Account.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Account.Domain.Repositories
{
    public interface IAuthenticationRepository
    {
        public Task<bool> AuthenticateUser(string userId, string password);
        public Task RemoveToken(string userId);

        public Task<RefreshTokenVM> GetRefreshToken(string userId);

        public Task UpdateToken(string userId, string refreshToken, DateTime expiresAt);
    }
}