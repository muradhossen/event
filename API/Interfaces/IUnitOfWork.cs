using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserReposetory UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikeRepository { get; }
        Task<bool> CompletedAsync();
        bool HasChanges();
    }
}
