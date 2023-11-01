using API.Interfaces;
using AutoMapper;
using System.Threading.Tasks;

namespace API.Data.Abstractions
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserReposetory UserRepository => new UserReposetory(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikeRepository => new LikesRepository(_context);

        public async Task<bool> CompletedAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
