﻿using Common.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace Common.Domain.Repositories
{
    public interface IFollowerRepository
    {
        Task AddFollower(string userId, Follower follower);
        Task<IEnumerable<Follower>> GetFollowersAsync(string userId);
        Task<IEnumerable<Follower>> GetUserFollowingAsync(string userId);
        Task<IActionResult> GetFollowerCountAsync(string userId);
        Task RemoveFollower(Follower follower);
    }
}