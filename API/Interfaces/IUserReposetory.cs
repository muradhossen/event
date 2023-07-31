using API.Dto;
using API.Entities;
using API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserReposetory
    {
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string name);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<bool> SaveAllAsync();
        void UpdateUser(AppUser user);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams pageParams);
        Task<MemberDto> GetMemberAsync(string Username);
    }
}
