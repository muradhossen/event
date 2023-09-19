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
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext
            , IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _dataContext.Messages
                .Include(c=> c.Sender)
                .Include(c=> c.Recipient)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _dataContext.Messages
                .OrderByDescending(c => c.MessageSend)
               .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username
                && m.RecipientDeleted == false),
                "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username
                && m.SenderDeleted == false),
                _ => query.Where(m => m.Recipient.UserName == messageParams.Username
                && m.RecipientDeleted == false && m.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);


            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageSize, messageParams.PageNumber);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _dataContext.Messages
                .Include(c => c.Recipient).ThenInclude(c => c.Photos)
                .Include(c => c.Sender).ThenInclude(c => c.Photos)
                 .Where(m =>
                        m.Sender.UserName == currentUsername && m.Recipient.UserName == recipientUsername && m.SenderDeleted == false
                        || m.Sender.UserName == recipientUsername && m.Recipient.UserName == currentUsername && m.RecipientDeleted == false)
                 .OrderBy(c => c.MessageSend)
                 .AsNoTracking()
                 .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
            && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages != null && unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                await _dataContext.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
