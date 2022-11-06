using Account.API.Common.ViewModels;
using Account.API.Infrastructure.Repositories;
using Account.API.Profile.Queries;
using Account.API.ViewModels;
using Account.Domain.Repositories;
using Account.Models.Users;
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Account.Tests.Profile.Queries
{
    public class ProfileQueriesTests
    {        
        [Fact]
        public async Task GetProfileInformation_ReturnsProfileInfoAsync()
        {
            User user = GetSampleUser();
            IEnumerable<Follower> followerList = Enumerable.Empty<Follower>();
            
            using (var mock = AutoMock.GetLoose())
            {
                //Mock repository and method within used in test
                mock.Mock<IUserRepository>()
                    .Setup(x => x.GetUserAsync(user.UserId))
                    .Returns(Task.FromResult(user));

                mock.Mock<IFollowerRepository>()
                    .Setup(x => x.GetFollowersAsync(user.UserId))
                    .Returns(Task.FromResult(followerList));

                //Create instance of class and call method
                var testClass = mock.Create<ProfileQueries>();
                var expected = new ProfileInformationVM
                {
                    Forename = user.Forename,
                    Surname = user.Surname,
                    ProfileDescription = user.ProfileDescription,
                    NumberOfFollowers = 0
                };

                var result = await testClass.GetProfileInformation(user.UserId) as ObjectResult;
                ProfileInformationVM? actual = result.Value as ProfileInformationVM; 
                
                Assert.Equivalent(expected, actual);
            }
        }

        [Fact]
        public async void GetUserFollowers_ReturnsProfileInfo()
        {
            string userId = GetSampleUser().UserId;
            IEnumerable<Follower> followers = GetSampleFollowers()
                                                .Where(f => f.UserId == userId);

            IEnumerable<User> userFollowers = GetSampleFollowers()
                                                .Where(f => f.UserId == userId)
                                                .Select(f => GetUser(f.FollowerId));

            using (var mock = AutoMock.GetLoose())
            {                
                //Mock repository and method within used in test
                mock.Mock<IFollowerRepository>()
                    .Setup(x => x.GetFollowersAsync(userId))
                    .Returns(Task.FromResult(followers));

                mock.Mock<IUserRepository>()
                    .When(() => true).Setup(x => x.GetUserAsync(userFollowers.ElementAt(0).UserId))
                    .Returns(Task.FromResult(GetUser(userFollowers.ElementAt(0).UserId)));

                mock.Mock<IUserRepository>()
                    .When(() => true).Setup(x => x.GetUserAsync(userFollowers.ElementAt(1).UserId))
                    .Returns(Task.FromResult(GetUser(userFollowers.ElementAt(1).UserId)));
                                
                //Create instance of class and call method
                var testClass = mock.Create<ProfileQueries>();
                                
                List<OkObjectResult> expected = new()
                {
                    new OkObjectResult(new UserDataVM
                    {
                        UserId = userFollowers.ElementAt(0).UserId,
                        Forename = userFollowers.ElementAt(0).Forename,
                        Surname = userFollowers.ElementAt(0).Surname
                    }),
                    new OkObjectResult(new UserDataVM
                    {
                        UserId = userFollowers.ElementAt(1).UserId,
                        Forename = userFollowers.ElementAt(1).Forename,
                        Surname = userFollowers.ElementAt(1).Surname
                    })
                };
                var actual = await testClass.GetUserFollowers(userId);
                
                Assert.Equivalent(expected, actual);
            }
        }

        [Fact]
        public void GetUserFollowers_ThrowsException()
        {
            string userId = GetSampleUser().UserId;

            using (var mock = AutoMock.GetLoose())
            {
                //Mock repository and method within used in test
                mock.Mock<IFollowerRepository>()
                    .Setup(x => x.GetFollowersAsync(userId))
                    .Returns(() => null);

                //Create instance of class
                var testClass = mock.Create<ProfileQueries>();

                Assert.ThrowsAsync<KeyNotFoundException>(async () => await testClass.GetProfileInformation(userId));
            }
        }

        [Fact]
        public async Task GetUserFollowing_ReturnsProfileInfoAsync()
        {
            string userId = GetSampleUser().UserId;
            IEnumerable<Follower> following = GetSampleFollowers()
                                                .Where(f => f.FollowerId == userId);

            IEnumerable<User> userFollowing = GetSampleFollowers()
                                                .Where(f => f.FollowerId == userId)
                                                .Select(f => GetUser(f.FollowerId));

            using (var mock = AutoMock.GetLoose())
            {
                //Mock repository and method within used in test
                mock.Mock<IFollowerRepository>()
                    .Setup(x => x.GetUserFollowingAsync(userId))
                    .Returns(Task.FromResult(following));

                mock.Mock<IUserRepository>()
                    .When(() => true).Setup(x => x.GetUserAsync(userFollowing.ElementAt(0).UserId))
                    .Returns(Task.FromResult(GetUser(userFollowing.ElementAt(0).UserId)));

                mock.Mock<IUserRepository>()
                    .When(() => true).Setup(x => x.GetUserAsync(userFollowing.ElementAt(1).UserId))
                    .Returns(Task.FromResult(GetUser(userFollowing.ElementAt(1).UserId)));

                mock.Mock<IUserRepository>()
                    .When(() => true).Setup(x => x.GetUserAsync(userFollowing.ElementAt(2).UserId))
                    .Returns(Task.FromResult(GetUser(userFollowing.ElementAt(2).UserId)));

                //Create instance of class and call method
                var testClass = mock.Create<ProfileQueries>();

                List<OkObjectResult> expected = new()
                {
                    new OkObjectResult(new UserDataVM
                    {
                        UserId = userFollowing.ElementAt(0).UserId,
                        Forename = userFollowing.ElementAt(0).Forename,
                        Surname = userFollowing.ElementAt(0).Surname
                    }),
                    new OkObjectResult(new UserDataVM
                    {
                        UserId = userFollowing.ElementAt(1).UserId,
                        Forename = userFollowing.ElementAt(1).Forename,
                        Surname = userFollowing.ElementAt(1).Surname
                    }),
                    new OkObjectResult(new UserDataVM
                    {
                        UserId = userFollowing.ElementAt(2).UserId,
                        Forename = userFollowing.ElementAt(2).Forename,
                        Surname = userFollowing.ElementAt(2).Surname
                    })
                };
                var actual = await testClass.GetUserFollowing(userId);

                Assert.Equivalent(expected, actual);
            }
        }

        [Fact]
        public void GetUserFollowing_ThrowsException()
        {
            string userId = GetSampleUser().UserId;

            using (var mock = AutoMock.GetLoose())
            {
                //Mock repository and method within used in test
                mock.Mock<IFollowerRepository>()
                    .Setup(x => x.GetUserFollowingAsync(userId))
                    .Returns(() => null);

                //Create instance of class
                var testClass = mock.Create<ProfileQueries>();

                Assert.ThrowsAsync<KeyNotFoundException>(async () => await testClass.GetUserFollowing(userId));
            }
        }

        private User GetSampleUser()
        {
            User sampleUser = new()
            {
                UserId = "1",
                Password = "password",
                Forename = "Phil",
                Surname = "Philington",
                ProfileDescription = "Descripto profilo"
            };

            return sampleUser;
        }

        private List<Follower> GetSampleFollowers()
        {
            List<Follower> followers = new()
            {
                new Follower
                {
                    UserId = "1",
                    FollowerId = "10"
                },
                new Follower
                {
                    UserId = "1",
                    FollowerId = "11"
                },
                new Follower
                {
                    UserId = "10",
                    FollowerId = "1"
                },
                new Follower
                {
                    UserId = "14",
                    FollowerId = "1"
                },
                new Follower
                {
                    UserId = "20",
                    FollowerId = "1"
                }
            };            

            return followers;
        }

        private User GetUser(string followerId)
        {
            List<User> followers = new()
            {
                new User
                {
                    UserId = "1",
                    Password = "password",
                    Forename = "Bob",
                    Surname = "Someone"
                },
                new User
                {
                    UserId = "10",
                    Password = "password",
                    Forename = "Other",
                    Surname = "Person"
                },
                new User
                {
                    UserId = "11",
                    Password = "password",
                    Forename = "Amir",
                    Surname = "Khan"
                },
                new User
                {
                    UserId = "14",
                    Password = "password",
                    Forename = "Ariel",
                    Surname = "Mermaidus"
                },
                new User
                {
                    UserId = "20",
                    Password = "password",
                    Forename = "Another",
                    Surname = "Name"
                }
            };

            int index = followers.FindIndex(u => u.UserId == followerId);
            return followers[index];            
        }
    }
}
