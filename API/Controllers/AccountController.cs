using API.Data;
using API.Dto;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext dataContext, ITokenService tokenService
            , IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await IsUserExist(registerDto.Username)) return BadRequest("User already exist.");

            if (string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Username))
                return BadRequest("User name or Password can't be empty");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();


            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return new UserDto
            {
                UserName = registerDto.Username,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _dataContext
                .Users.Include(x => x.Photos)
                .SingleOrDefaultAsync(c => c.UserName == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            var result = new UserDto
            {
                UserName = loginDto.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain == true)?.Url,
                KnownAs = user.KnownAs
            };

            return result;
        }

        private async Task<bool> IsUserExist(string userName)
        {
            return await _dataContext.Users.AnyAsync(x => x.UserName == userName.ToLower());

        }
    }
}