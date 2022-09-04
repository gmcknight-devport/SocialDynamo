using Account.Infrastructure.Persistence;
using Account.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _accountDbContext;

        public UserRepository(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public async Task AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _accountDbContext.Users.Add(user);
            await _accountDbContext.SaveChangesAsync();
        }
        public async Task DeleteUser(int userId)
        {
            var user = await _accountDbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _accountDbContext.Users.Remove(user);
            await _accountDbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var user = await _accountDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user;
        }

        public async Task<User> GetUserAsync(string emailAddress)
        {
            List<User> user = await _accountDbContext.Users
                .Include(u => u.EmailAddress)                
                .Where(u => u.EmailAddress == emailAddress).ToListAsync();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return user.ElementAt(0);
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
    }
}
