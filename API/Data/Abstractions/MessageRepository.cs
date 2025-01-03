﻿using API.Dto;
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

        public void AddGroup(Group group)
        {
            _dataContext.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _dataContext.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            var group =await _dataContext.Groups
                .Include(c => c.Connections)
                .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();

            return group;
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _dataContext.Messages
                .Include(c => c.Sender)
                .Include(c => c.Recipient)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _dataContext.Groups
                  .Include(g => g.Connections)
                  .FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _dataContext.Messages
                .OrderByDescending(c => c.MessageSend)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
               .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m => m.RecipientUsername == messageParams.Username
                && m.RecipientDeleted == false),
                "Outbox" => query.Where(m => m.SenderUsername== messageParams.Username
                && m.SenderDeleted == false),
                _ => query.Where(m => m.RecipientUsername == messageParams.Username
                && m.RecipientDeleted == false && m.DateRead == null)
            };

             

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageSize, messageParams.PageNumber);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _dataContext.Messages                 
                 .Where(m =>
                        m.Sender.UserName == currentUsername && m.Recipient.UserName == recipientUsername && m.SenderDeleted == false
                        || m.Sender.UserName == recipientUsername && m.Recipient.UserName == currentUsername && m.RecipientDeleted == false)
                 .OrderBy(c => c.MessageSend)
                 .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                 .AsNoTracking()
                 .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
            && m.RecipientUsername == currentUsername).ToList();

            if (unreadMessages != null && unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                 
            }
            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _dataContext.Connections.Remove(connection);
        }
         
    }
}
