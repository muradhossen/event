﻿using API.Data.Abstractions;
using API.Dto;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class MessageHub : Hub
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;

        public MessageHub(IMapper mapper
            , IUnitOfWork unitOfWork
            , IHubContext<PresenceHub> presenceHub
            , PresenceTracker tracker)
        {

            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _presenceHub = presenceHub;
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            if (_unitOfWork.HasChanges())
            {
                await _unitOfWork.CompletedAsync();
            }

            await Clients.Caller.SendAsync("ReciveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            string userName = Context.User.GetUserName();
            if (userName == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot message yourself");

            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(userName);
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                throw new HubException("Recipient not found!");
            }
            var message = new Message
            {
                Sender = sender,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Recipient = recipient,
                Content = createMessageDto.Content,
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(c => c.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recipient.UserName);

                if (connections is not null)
                {
                    await _presenceHub.Clients.Clients(connections)
                        .SendAsync("NewMessageRecived", new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.CompletedAsync())
            {

                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

            }
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (group is null)
            {
                group = new Group(groupName);
                _unitOfWork.MessageRepository.AddGroup(group);

            }
            group.Connections.Add(connection);

            if (await _unitOfWork.CompletedAsync())
            {
                return group;
            }

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);

            var connection = group.Connections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            _unitOfWork.MessageRepository.RemoveConnection(connection);
            if (await _unitOfWork.CompletedAsync())
            {
                return group;
            }

            throw new HubException("Failed to remove connection");
        }
    }
}
