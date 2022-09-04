using Account.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Infrastructure.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AccountDbContext _accountDbContext;

        public AuthenticationRepository(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task<bool> AuthenticateUser(int userId, string password)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);
            if (user != null &&  user.UserId == userId && user.Password == password)
            {
                return true;
            }
            return false;
        }
        public async Task RemoveToken(int userId)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);

            if (user != null)
            {
                user.RefreshToken = null;
                await _accountDbContext.SaveChangesAsync();
            }
        }
                
        public async Task UpdateToken(int userId, string refreshToken, DateTime expiresAt)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);

            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshExpires = expiresAt;
                await _accountDbContext.SaveChangesAsync();
            }
        }

        public async Task<IActionResult?> GetRefreshToken(int userId)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);

            if (user != null)
            {
                return new OkObjectResult(new Tuple<string, DateTime>
                (
                    user.RefreshToken,
                    user.RefreshExpires
                ));
            }
            return null;
        }
    }
}
