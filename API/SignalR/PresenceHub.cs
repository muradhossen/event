using API.Extentions;
using API.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public PresenceHub(PresenceTracker tracker,
            IUnitOfWork unitOfWork)
        {
            _tracker = tracker;
            _unitOfWork = unitOfWork;
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

           var messages = await _unitOfWork.UserPhotoMessageRepository.GetAllUserImageMessages();

            await Clients.Caller.SendAsync("GettAllPhotoMessages", messages);


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
