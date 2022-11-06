using Account.Domain.Repositories;
using Account.Domain.ViewModels;
using Account.Infrastructure.Persistence;
using Account.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Infrastructure.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AccountDbContext _accountDbContext;

        public AuthenticationRepository(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task<bool> AuthenticateUser(string userId, string password)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);
            if (user != null && user.UserId == userId && user.Password == password)
            {
                return true;
            }
            return false;
        }
        public async Task RemoveToken(string userId)
        {
            var user = await GetUserAsync(userId);
                        
            user.RefreshToken = string.Empty;
            user.RefreshExpires = DateTime.UtcNow.AddDays(-1);

            await _accountDbContext.SaveChangesAsync();            
        }
                
        public async Task UpdateToken(string userId, string refreshToken, DateTime expiresAt)
        {
            var user = await GetUserAsync(userId);
                        
            user.RefreshToken = refreshToken;
            user.RefreshExpires = expiresAt;
           
            await _accountDbContext.SaveChangesAsync();            
        }

        public async Task<RefreshTokenVM> GetRefreshToken(string userId)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NullReferenceException(nameof(user));                
            }

            return new RefreshTokenVM
            {
                RefreshToken = user.RefreshToken,
                RefreshExpires = user.RefreshExpires
            };
        }

        private async Task<User> GetUserAsync(string userId)
        {
            var user = await _accountDbContext.Users.Include(x => x.Followers).FirstOrDefaultAsync(x => x.UserId == userId);
            if(user == null)
            {
                throw new NullReferenceException(nameof(user));
            }

            return user;
        }
    }
}
