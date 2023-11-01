using API.Dto;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Abstractions
{
    public class UserReposetory : IUserReposetory
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserReposetory(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string Username)
        {
            return await _context.Users.Where(x=>x.UserName.ToLower()==Username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams pageParams)
        {
            var query = _context
                .Users
               .AsQueryable();

            query = query.Where(c => c.UserName != pageParams.Username);
            query = query.Where(c => c.Gender.ToLower() == pageParams.Gender);

            var minDob = DateTime.Now.AddYears(-pageParams.MaxAge-1);
            var maxDob = DateTime.Now.AddYears(-pageParams.MinAge);

            query = query.Where(c=> c.DateOfBirth >= minDob &&  c.DateOfBirth <= maxDob);

            query = pageParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(
                query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                pageParams.PageSize, pageParams.PageNumber);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string name)
        {
            return await _context.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == name);
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                .Where(c => c.UserName == username)
                .Select(c => c.Gender)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

 

        public void UpdateUser(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
