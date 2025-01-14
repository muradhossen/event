using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<UserImageMessage> AddAsync(UserImageMessage userImageMessage)
    {
        await _dbContext.UserImageMessages.AddAsync(userImageMessage);

        return userImageMessage;
    }

    public async Task<List<UserImageMessage>>  GetAllAsync()
    {
        return await _dbContext.UserImageMessages.OrderByDescending(c => c.Id).ToListAsync();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            return false;
        }

        var userImageMessage = await _dbContext.UserImageMessages.FindAsync(id);

        if (userImageMessage == null)
        {
            return false;
        }
        _dbContext.UserImageMessages.Remove(userImageMessage);

        return true;
    }
}
