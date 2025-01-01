using API.Data;
using API.Dto;
using API.Entities;
using API.Extentions;
using API.Helpers;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUnitOfWork unitOfWork
            , IMapper mapper
            , IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> Get([FromQuery] UserParams pageParams)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUserName());

            pageParams.Username = User.GetUserName();

            if (string.IsNullOrWhiteSpace(pageParams.Gender))
            {
                pageParams.Gender = gender.ToLower() == "Male".ToLower() ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMembersAsync(pageParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage);

            return Ok(users);
        }


        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetByUser(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {

            var member = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            _mapper.Map(memberUpdateDto, member);
            _unitOfWork.UserRepository.UpdateUser(member);
            if (await _unitOfWork.CompletedAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update user");
        }

        [HttpPost("Add-Photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

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
            if (await _unitOfWork.CompletedAsync())
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            return BadRequest("Failled to upload image");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            if (photoId < 0) return BadRequest("Invalid Input");

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());


            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo != null && photo.IsMain == true) return BadRequest("This is already your main photo");

            var current = user.Photos.FirstOrDefault(x => x.IsMain == true);

            if (current != null) current.IsMain = false;
            photo.IsMain = true;
            if (await _unitOfWork.CompletedAsync())
                return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            if (photoId < 0) return BadRequest("Invalid Input");

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.IsMain) return BadRequest("Cann't delete main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeleteAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _unitOfWork.CompletedAsync()) return Ok();

            return BadRequest("Failed to delete photo");
        }

        [HttpPost("upload-photo")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto([FromForm] UserPhotoParam @params)
        {

            var result = await _photoService.AddPhotoAsync(@params.Image);

            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            }; 
            return Ok(_mapper.Map<PhotoDto>(photo));

        }
    }
}
