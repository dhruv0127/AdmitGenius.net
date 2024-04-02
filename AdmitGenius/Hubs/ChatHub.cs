using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdmitGenius.Data;
using AdmitGenius.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AdmitGenius.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AdmitDBContext dbContext;

        public ChatHub(AdmitDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private const int MessagesPerPage = 10;
        private int messagesLoaded = 0;


        public async Task JoinGroupww(string groupName)
        {
            var connectionId = Context.ConnectionId;
            var user = GetUserFromSession();

            if (user != null)
            {
                // Get chat history for the group from the database
                var chatHistory = dbContext.ChatMessages
                    .Where(m => m.groupName == groupName)
                    .OrderBy(m => m.Timestamp)
                    .ToList();

                // Send chat history to the user who joined
                foreach (var message in chatHistory)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
                }

                // Add the user to the group
                await Groups.AddToGroupAsync(connectionId, groupName);

                // Notify the group that the user has joined
                //await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{user.UserName} has joined the group {groupName}");
            }
        }


        public async Task JoinGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;
            var user = GetUserFromSession();

            if (user != null)
            {
                // Get the total number of messages in the group
                var totalMessages = dbContext.ChatMessages.Count(m => m.groupName == groupName);

                // Load the last 20 messages
                var chatHistory = dbContext.ChatMessages
                    .Where(m => m.groupName == groupName)
                    .OrderByDescending(m => m.Timestamp)
                    .Take(MessagesPerPage)
                    .OrderBy(m => m.Timestamp)
                    .ToList();


                var totalMessagesFOR = dbContext.ChatMessages.Count(m => m.groupName == groupName);

                // Send chat history to the user who joined
                foreach (var message in chatHistory)
                {
                    if (message.ContentType == false)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
                    }
                    else
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveImage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
                    }
                }
                var totalMessagesFOR1 = dbContext.ChatMessages.Count(m => m.groupName == groupName);

                messagesLoaded = chatHistory.Count;

                // Send total number of messages to the client
                await Clients.Client(connectionId).SendAsync("SetTotalMessages", totalMessages);

                // Add the user to the group
                await Groups.AddToGroupAsync(connectionId, groupName);
            }
        }

        public async Task LoadMoreMessagesaa(string groupName)
        {
            var connectionId = Context.ConnectionId;

            var additionalMessages = dbContext.ChatMessages
                .Where(m => m.groupName == groupName)
                .OrderByDescending(m => m.Timestamp)
                .Skip(messagesLoaded)
                .Take(MessagesPerPage)
                .OrderBy(m => m.Timestamp)
                .ToList();

            messagesLoaded += additionalMessages.Count;

            // Send additional messages to the user
            foreach (var message in additionalMessages)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
            }
        }

        public async Task LoadMoreMessages(string groupName, int messagesToLoad)
        {
            var connectionId = Context.ConnectionId;

            var totalMessages = dbContext.ChatMessages.Count(m => m.groupName == groupName);

            var startingPoint = Math.Max(0, totalMessages - messagesToLoad);

            var additionalMessages = dbContext.ChatMessages
                .Where(m => m.groupName == groupName)
                .OrderBy(m => m.Timestamp)
                .Skip(startingPoint)
                .Take(messagesToLoad)
                .ToList();

            // Send additional messages to the user
            foreach (var message in additionalMessages)
            {

                if (message.ContentType == false)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveImage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
                }

                //cahnge here
                //await Clients.Client(connectionId).SendAsync("ReceiveMessage", message.Sender, message.UserProfile, message.Message, message.Timestamp);
            }
        }

        public async Task<int> GetTotalMessagesCount(string groupName)
        {
            var totalMessages = dbContext.ChatMessages.Count(m => m.groupName == groupName);
            return totalMessages;
        }





        public async Task LeaveGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;

            // Clear previous group's messages for the user
            await Clients.Client(connectionId).SendAsync("ClearMessages");

            // Remove the user from the group
            await Groups.RemoveFromGroupAsync(connectionId, groupName);

            // Notify the group that the user has left
            //await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the group {groupName}");
        }


        public async Task SendMessageToGroup(string groupName, string message)
        {
            var user = GetUserFromSession();



            if (user != null)
            {
                var userName = user.UserName;
                var userProfilePicture = user.UserProfile;

                var chatMessage = new ChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Sender = userName,
                    Message = message,
                    Timestamp = DateTime.Now,
                    UserProfile = userProfilePicture,
                    groupName = groupName,
                    //content type false for text messages
                    ContentType = false
                };

                dbContext.ChatMessages.Add(chatMessage);
                await dbContext.SaveChangesAsync();

                await Clients.Group(groupName).SendAsync("ReceiveMessage", userName, userProfilePicture, message, chatMessage.Timestamp);

                var community = dbContext.Community.SingleOrDefault(c => c.CommunityId.ToString() == groupName);
                if (community != null)
                {
                    community.LastMessage = message;
                    await dbContext.SaveChangesAsync();
                }

                var CounselorSubscribers = dbContext.CounselorSubscribers.SingleOrDefault(c => c.GroupName == groupName);
                if (CounselorSubscribers != null)
                {
                    CounselorSubscribers.LastMessage = message;
                    await dbContext.SaveChangesAsync();
                }
            }


        }

        public async Task SendImageToGroup(string groupName, string message)
        {
            var user = GetUserFromSession();

            if (user != null)
            {
                var userName = user.UserName;
                var userProfilePicture = user.UserProfile;

                var chatMessage = new ChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Sender = userName,
                    Message = message,
                    Timestamp = DateTime.Now,
                    UserProfile = userProfilePicture,
                    groupName = groupName,
                    //content type true for image messages
                    ContentType = true
                };

                dbContext.ChatMessages.Add(chatMessage);
                await dbContext.SaveChangesAsync();

                await Clients.Group(groupName).SendAsync("ReceiveImage", userName, userProfilePicture, chatMessage.Message, chatMessage.Timestamp);

                var community = dbContext.Community.SingleOrDefault(c => c.CommunityId.ToString() == groupName);
                if (community != null)
                {
                    community.LastMessage = "Image File";
                    await dbContext.SaveChangesAsync();
                }

                var CounselorSubscribers = dbContext.CounselorSubscribers.SingleOrDefault(c => c.GroupName == groupName);
                if (CounselorSubscribers != null)
                {
                    CounselorSubscribers.LastMessage = "Image File";
                    await dbContext.SaveChangesAsync();
                }
            }
        }


        public async Task SendImageToGroupold(string groupName, string base64Image)
        {
            var user = GetUserFromSession();

            if (user != null)
            {
                var userName = user.UserName;
                var userProfilePicture = user.UserProfile;

                var chatMessage = new ChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Sender = userName,
                    Message = "imagefiile",
                    Timestamp = DateTime.Now,
                    UserProfile = userProfilePicture,
                    groupName = groupName,
                    ContentType = true
                };

                dbContext.ChatMessages.Add(chatMessage);
                await dbContext.SaveChangesAsync();

                // Send the base64Image to clients
                await Clients.Group(groupName).SendAsync("ReceiveImage", userName, userProfilePicture, base64Image, chatMessage.Timestamp);

                var community = dbContext.Community.SingleOrDefault(c => c.CommunityId.ToString() == groupName);
                if (community != null)
                {
                    community.LastMessage = "Image";
                    await dbContext.SaveChangesAsync();
                }

                var CounselorSubscribers = dbContext.CounselorSubscribers.SingleOrDefault(c => c.GroupName == groupName);
                if (CounselorSubscribers != null)
                {
                    CounselorSubscribers.LastMessage = "Image";
                    await dbContext.SaveChangesAsync();
                }
            }
        }




        private Users GetUserFromSession()
        {
            var userIdString = Context.GetHttpContext().Session.GetString("UserId");

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                var user = dbContext.Users.Find(userId);
                return user;
            }

            return null;
        }


    }
}
