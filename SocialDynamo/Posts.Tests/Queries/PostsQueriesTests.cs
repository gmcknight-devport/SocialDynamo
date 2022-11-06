using Autofac.Extras.Moq;
using Posts.API.Queries;
using Posts.Domain.Models;
using Posts.Domain.Repositories;
using Posts.Domain.ValueObjects;
using Posts.Domain.ViewModels;
using Posts.Infrastructure.Repositories;
using Xunit;

namespace Posts.Tests.Queries
{
    public class PostsQueriesTests
    {
        [Fact]
        public async Task GetUserPosts_ReturnsSpecifiedUserPosts()
        {
            List<Post> posts = GetSampleData().ToList();
            string userId = posts.ElementAt(0).AuthorId;

            List<Post> expected = new();
            foreach (Post post in posts)
            {
                if (post.AuthorId == userId)
                {
                    expected.Add(post);
                }
            }

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(x => x.GetUserPostsAsync(userId, 1))
                    .Returns(Task.FromResult(expected.AsEnumerable()));

                var testClass = mock.Create<PostsQueries>();

                var actual = await testClass.GetUserPostsAsync(userId, 1);

                Assert.Equivalent(expected, actual);
            }
        }

        [Fact]
        public async Task GetUsersPosts_ReturnsMultipleSpecifiedUsersPosts()
        {
            List<Post> posts = GetSampleData().ToList();
            List<string> userIds = new()
            {
                posts.ElementAt(0).AuthorId,
                posts.ElementAt(2).AuthorId
            };

            List<Post> expected = new();
            foreach (Post post in posts)
            {
                foreach (string id in userIds)
                {
                    if (post.AuthorId == id)
                    {
                        expected.Add(post);
                    }
                }
            }

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(x => x.GetUsersPostsAsync(userIds, 1))
                    .Returns(Task.FromResult(expected.AsEnumerable()));

                var testClass = mock.Create<PostsQueries>();

                var actual = await testClass.GetUsersPostsAsync(userIds, 1);

                Assert.Equivalent(expected, actual);
            }
        }

        [Fact]
        public async Task GetPostComments_AllCommentsFromSpecificPost()
        {
            Post post = GetSampleData().ElementAt(1);

            List<Comment> comments = post.Comments.ToList();
            List<CommentVM> expected = new();

            foreach (var v in comments)
            {
                expected.Add(new CommentVM
                {
                    AuthorId = v.AuthorId,
                    CommentId = v.CommentId,
                    CommentText = v.CommentText,
                    LikeCount = v.Likes.Count,
                    PostedAt = v.PostedAt
                });
            }

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ICommentRepository>()
                    .Setup(x => x.GetPostCommentsAsync(post.PostId, 1))
                    .Returns(Task.FromResult(expected.AsEnumerable()));

                var testClass = mock.Create<PostsQueries>();

                var actual = await testClass.GetPostCommentsAsync(post.PostId, 1);

                Assert.Equivalent(comments, actual);
            }
        }

        [Fact]
        public async Task GetCommentLikes_AllLikesFromSpecificComment()
        {
            Post post = GetSampleData().ElementAt(1);
            Comment comment = post.Comments.ElementAt(0);
            List<CommentLike> comments = comment.Likes.ToList();

            List<LikeVM> expected = new();

            foreach (var v in comments)
            {
                expected.Add(new LikeVM
                {
                    Id = v.Id,
                    LikeUserId = v.LikeUserId
                });
            }

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ICommentRepository>()
                    .Setup(x => x.GetCommentLikesAsync(comment.CommentId))
                    .Returns(Task.FromResult(expected.AsEnumerable()));

                var testClass = mock.Create<PostsQueries>();

                var actual = await testClass.GetCommentLikesAsync(comment.CommentId);

                Assert.Equivalent(comments, actual);
            }
        }

        [Fact]
        public async Task GetPostLikes_AllLikesFromSpecificPost()
        {
            Post post = GetSampleData().ElementAt(1);
            List<PostLike> comments = post.Likes.ToList();

            List<LikeVM> expected = new();

            foreach (var v in comments)
            {
                expected.Add(new LikeVM
                {
                    Id = v.Id,
                    LikeUserId = v.LikeUserId
                });
            }

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IPostRepository>()
                    .Setup(x => x.GetPostLikesAsync(post.PostId))
                    .Returns(Task.FromResult(expected.AsEnumerable()));

                var testClass = mock.Create<PostsQueries>();

                var actual = await testClass.GetPostLikesAsync(post.PostId);

                Assert.Equivalent(expected, actual);
            }
        }

        private ICollection<Post> GetSampleData()
        {
            List<Post> posts = new();

            Post post1 = new()
            {
                AuthorId = "11",
                Caption = "Caption test val",
                PostedAt = DateTime.Now.AddDays(-1),
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create("11"),
                    MediaItemId.Create("11")
                },
                Likes = new List<PostLike>(),
                Comments = new List<Comment>(),
            };

            Post post2 = new()
            {
                AuthorId = "11",
                Caption = "Another lovely caption",
                PostedAt = DateTime.Now.AddDays(-2),
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create("11"),
                    MediaItemId.Create("11")
                },
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            Post post3 = new()
            {
                AuthorId = "11",
                Caption = "A caption I can't believe",
                PostedAt = DateTime.Now.AddDays(-3),
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create("11"),
                    MediaItemId.Create("11")
                },
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            Post post4 = new()
            {
                AuthorId = "22",
                Caption = "A different user with a post",
                PostedAt = DateTime.Now.AddDays(-1),
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create("22"),
                    MediaItemId.Create("22")
                },
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            Post post5 = new()
            {
                AuthorId = "22",
                Caption = "A different user with a second post",
                PostedAt = DateTime.Now,
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create("22"),
                    MediaItemId.Create("22")
                },
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            Post post6 = new()
            {
                AuthorId = "33",
                Caption = "A third user?!?",
                PostedAt = DateTime.Now.AddDays(-1),
                MediaItemIds = new List<MediaItemId>()
                {
                    MediaItemId.Create("33"),
                    MediaItemId.Create("33")
                },
                Likes = new List<PostLike>(),
                Comments = new List<Comment>()
            };

            Comment comment1 = new()
            {
                AuthorId = post2.AuthorId,
                PostedAt = DateTime.UtcNow,
                CommentText = "First comment yeah",
                Likes = new List<CommentLike>(),
                Post = post1
            };

            Comment comment2 = new()
            {
                AuthorId = post2.AuthorId,
                PostedAt = DateTime.UtcNow,
                CommentText = "Second comment, ooh",
                Likes = new List<CommentLike>(),
                Post = post1
            };

            PostLike postLike1 = new()
            {
                LikeUserId = "11",
                Post = post2
            };

            PostLike postLike2 = new()
            {
                LikeUserId = "11",
                Post = post1
            };

            PostLike postLike3 = new()
            {
                LikeUserId = "22",
                Post = post2
            };

            CommentLike commentLike1 = new()
            {
                LikeUserId = "11",
                Comment = comment1
            };

            CommentLike commentLike2 = new()
            {
                LikeUserId = "22",
                Comment = comment1
            };

            CommentLike commentLike3 = new()
            {
                LikeUserId = "11",
                Comment = comment2
            };

            post2.Comments.Add(comment1);
            post2.Comments.Add(comment2);

            post2.Likes.Add(postLike1);
            post2.Likes.Add(postLike3);
            post1.Likes.Add(postLike2);

            comment1.Likes.Add(commentLike1);
            comment1.Likes.Add(commentLike2);
            comment2.Likes.Add(commentLike3);

            posts.Add(post1);
            posts.Add(post2);
            posts.Add(post3);
            posts.Add(post4);
            posts.Add(post5);
            posts.Add(post6);

            posts.OrderByDescending(x => x.PostedAt);

            return posts;
        }
    }
}
