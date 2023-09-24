using Account.Domain.Repositories;
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

        public async Task<bool> IsUserIdUnique(string userId)
        {
            var existsInDatabase = await _accountDbContext.Users.FindAsync(userId);
            if (existsInDatabase == null)
                return true;

            return false;
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
            var user = await GetUserAsync(userId);            
            _accountDbContext.Users.Remove(user);
            await _accountDbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var user = await _accountDbContext.Users.Include(x => x.Followers).FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string emailAddress)
        {
            var user = await _accountDbContext.Users.Include(x => x.Followers).FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);

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
            var results = await _accountDbContext.Users.Where(d =>
                                    EF.Functions.Like(d.UserId, "%" + fuzzyUserId + "%"))
                                    .Take(5).ToListAsync();

            return results;
        }
    }
}
