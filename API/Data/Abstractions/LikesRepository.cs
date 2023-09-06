using API.Dto;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Abstractions
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _dbContext;

        public LikesRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int likeUserId)
        {
            return await _dbContext.Likes.FindAsync(sourceUserId, likeUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikesAsync(LikeParams likeParams)
        {
            var users = _dbContext.Users.OrderBy(c => c.UserName).AsQueryable();
            var likes = _dbContext.Likes.AsQueryable();

            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(user => user.LikedUser);
            }

            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
                users = likes.Select(user => user.SourceUser);
            }

            var members = users.Select(c => new LikeDto
            {
                Age = c.DateOfBirth.CalculateAge(),
                City = c.City,
                Id = c.Id,
                KnownAs = c.KnownAs,
                PhotoUrl = c.Photos.FirstOrDefault(c=> c.IsMain).Url,
                Username = c.UserName,
            });

            return await PagedList<LikeDto>.CreateAsync(members, likeParams.PageSize, likeParams.PageNumber);
        }

        public async Task<AppUser> GetUserWithLikesAsync(int userId)
        {
            return await _dbContext.Users
                .Include(c => c.LikedUsers)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
