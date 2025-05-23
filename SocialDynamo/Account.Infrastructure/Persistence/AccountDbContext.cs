﻿using Common.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure.Persistence
{
    public class AccountDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Follower> Followers { get; set; }

        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {        

        }
    }
}
