using API.Dto;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly IUserReposetory _userReposetory;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserReposetory userReposetory
            , ILikesRepository likesRepository)
        {
            _userReposetory = userReposetory;
            _likesRepository = likesRepository;
        }

        [HttpPost("{userName}")]
        public async Task<IActionResult> AddLike(string userName)
        {
            int sourceUserId = User.GetUserId();

            var likedUser = await _userReposetory.GetUserByUsernameAsync(userName);
            var sourceUser = await _likesRepository.GetUserWithLikesAsync(sourceUserId);

            if (likedUser == null) { return NotFound(); }

            if (sourceUser.UserName == userName) { return BadRequest("You cannot like yourself!"); }

            var userLike = await _likesRepository.GetUserLikeAsync(sourceUserId, likedUser.Id);

            if (userLike != null) { return BadRequest("You already like this user."); }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _userReposetory.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Failed to save user like");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {

            likeParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikesAsync(likeParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount,users.TotalPage);

            return Ok(users);
        }
    }
}
