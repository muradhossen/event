﻿using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            bool isOnline = await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);

            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
            }

            var onlineUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", onlineUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            bool isOffline = await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);

            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());
            }



            await base.OnDisconnectedAsync(exception);
        }
    }
}
