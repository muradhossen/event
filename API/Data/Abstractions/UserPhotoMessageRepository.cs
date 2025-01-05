using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data.Abstractions;

public class UserPhotoMessageRepository : IUserPhotoMessageRepository
{
    private readonly DataContext _dbContext;

    public UserPhotoMessageRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IList<UserImageMessage>> GetAllUserImageMessages()
    {
        return await _dbContext.UserImageMessages.ToListAsync();
    }
}
