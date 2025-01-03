﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {

        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();
        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool isOnline = false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string>() { connectionId });
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOfflline = false;
            lock (OnlineUsers)
            {

                if (!OnlineUsers.ContainsKey(username))
                {
                    return Task.FromResult(isOfflline);
                }

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOfflline = true;
                }
                return Task.FromResult(isOfflline);
            }
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(u => u.Key).Select(u => u.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connections;
            lock (OnlineUsers)
            {
                connections = OnlineUsers
                              .Where(c => c.Key == username)
                              .SelectMany(c => c.Value)
                              .ToList();
            }

            return Task.FromResult(connections);
        }

        public Task<List<string>> GetAllConnections()
        {
            List<string> connections;
            lock (OnlineUsers)
            {
                connections = OnlineUsers 
                              .SelectMany(c => c.Value)
                              .ToList();
            }

            return Task.FromResult(connections);
        }
    }
}
