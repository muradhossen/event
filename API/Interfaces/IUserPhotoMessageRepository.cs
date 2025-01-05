using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces;

public interface IUserPhotoMessageRepository
{
    Task<IList<UserImageMessage>> GetAllUserImageMessages();
}
