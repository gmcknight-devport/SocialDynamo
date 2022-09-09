using Account.Infrastructure.Persistence;
using Account.Models.Users;
using Common;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository, IFuzzySearch
    {
        private readonly AccountDbContext _accountDbContext;

        public UserRepository(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _accountDbContext.Users.Add(user);
            await _accountDbContext.SaveChangesAsync();
        }
        public async Task DeleteUserAsync(string userId)
        {
            var user = await _accountDbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _accountDbContext.Users.Remove(user);
            await _accountDbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user;
        }
        
        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _accountDbContext.Users.Update(user);
            await _accountDbContext.SaveChangesAsync();
        }        

        public async Task<IEnumerable<object>> FuzzySearch(string fuzzyUserId)
        {
            var results = _accountDbContext.Users.Where(d => EF.Functions.FreeText(d.UserId, fuzzyUserId));
            return results;
        }
    }
}
