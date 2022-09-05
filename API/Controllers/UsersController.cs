using API.Data;
using API.Dto;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserReposetory _userReposetory;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserReposetory userReposetory, IMapper mapper,
            IPhotoService photoService)
        {
            _userReposetory = userReposetory;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> Get()
        {
            var users = await _userReposetory.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetByUser(string username)
        {
            return await _userReposetory.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {

            var member = await _userReposetory.GetUserByUsernameAsync(User.GetUserName());

            _mapper.Map(memberUpdateDto, member);
            _userReposetory.UpdateUser(member);
            if (await _userReposetory.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update user");
        }

        [HttpPost("Add-Photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userReposetory.GetUserByUsernameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await _userReposetory.SaveAllAsync())
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            return BadRequest("Failled to upload image");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            if (photoId < 0) return BadRequest("Invalid Input");

            var user= await _userReposetory.GetUserByUsernameAsync(User.GetUserName());


            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo != null && photo.IsMain == true) return BadRequest("This is already your main photo");  

            var current = user.Photos.FirstOrDefault(x=>x.IsMain == true);
           
            if (current != null) current.IsMain = false;
            photo.IsMain = true;
            if (await _userReposetory.SaveAllAsync())
                return NoContent();

            return BadRequest("Failled to set main photo");
        }
    }
}
