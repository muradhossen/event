using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces;

public interface IUserPhotoMessageRepository
{
    Task<IList<UserImageMessage>> GetAllUserImageMessages();
    Task<UserImageMessage> AddAsync(UserImageMessage userImageMessage);
    Task<List<UserImageMessage>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}
