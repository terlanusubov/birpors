using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.API.Chat
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CookHub : Hub
    {
        private readonly ChatClientManager clientManager;
        private readonly IApplicationDbContext context;

        public CookHub(ChatClientManager _clientManager, IApplicationDbContext _context)
        {
            clientManager = _clientManager;
            context = _context;
        }

        //public override async Task OnConnectedAsync()
        //{
        //    await context.AppLogs.AddAsync(new AppLog
        //    {
        //        Created = DateTime.Now,
        //        LogText = "negotiate geldi",
        //        LogType = 10
        //    });

        //    await context.SaveChanges();
        //    var negotiateResponse = new
        //    {
        //        ConnectionId = Context.ConnectionId,
        //        Url = $"https://birpors.azurewebsites.net/CookHub",
        //    };

        //    await Clients.Client(Context.ConnectionId).SendAsync("negotiate", negotiateResponse);
        //    await base.OnConnectedAsync();
        //}

        public async Task Join()
        {
            await context.AppLogs.AddAsync(new AppLog
            {
                Created = DateTime.Now,
                LogText = "Connection yarandi..." + Context.ConnectionId + " " + Context.UserIdentifier,
                LogType = 10
            });
            await context.SaveChanges();

            string connectionId = Context.ConnectionId;
            var userClaim = Context.User.Claims.Where(c => c.Type == "userId").FirstOrDefault();
            string userId = userClaim.Value;

            if (userId != null && connectionId != null)
            {
                if (await clientManager.IsJoinedAsync(userId))
                    await clientManager.RemoveAsync(userId);

                await clientManager.AddAsync(userId, connectionId);
            }
        }

        // Chat sehifesine daxil olanda
        public async Task Online()
        {
            // if (await clientManager.IsJoinedAsync(Context.UserIdentifier))
            //     await clientManager.ChangeOnlineStatus(Context.UserIdentifier, true);
        }

        // Chat sehifesinden cixanda
        public async Task Offline()
        {
            // if (await clientManager.IsJoinedAsync(Context.UserIdentifier))
            //     await clientManager.ChangeOnlineStatus(Context.UserIdentifier, false);
        }

        public async Task SendMessage(string text, string conversationId)
        {

            await context.AppLogs.AddAsync(new AppLog
            {
                Created = DateTime.Now,
                LogText = text + " " + conversationId,
                LogType = 10
            });

            await context.SaveChanges();

            if (text != null && conversationId != null)
            {
                var userClaim = Context.User.Claims.Where(c => c.Type == "userId").FirstOrDefault();
                string userId = userClaim.Value;
                if (userId != null)
                {
                    Conversation conversation = await context.Conversations
                        .Include(c => c.Participants)
                        .FirstOrDefaultAsync(c => c.Id == Convert.ToInt32(conversationId));

                    if (conversation != null)
                    {
                        int messageTo = (int)conversation.Participants
                            .FirstOrDefault(c => c.AppUserId != Convert.ToInt32(userId))
                            ?.AppUserId;

                        if (messageTo != 0)
                        {
                            Message message = new Message
                            {
                                SenderId = Convert.ToInt32(userId),
                                Text = text,
                                CreatedDate = DateTime.Now,
                                ConversationId = conversation.Id,
                                Guid = Guid.NewGuid().ToString(),
                            };

                            await context.Messages.AddAsync(message);
                            await context.SaveChanges();

                            Client sender = await clientManager.GetClientAsync(userId);
                            Client receiver = await clientManager.GetClientAsync(messageTo.ToString());

                            var data = new
                            {
                                text = text,
                                conversationId = conversationId
                            };

                            //receive message cagirilan zaman eger true dursa sagda false dursa solda gorsensin mesaj
                            await Clients
                                .Client(sender.ConnectionId)
                                .SendAsync("ReceiveMessage", data, true);

                            if (receiver != null)
                            {

                                await Clients
                                    .Client(receiver.ConnectionId)
                                    .SendAsync("ReceiveMessage", data, false);

                            }
                        }
                    }
                }
            }
        }

    }

    //public async Task MakeConversationViewable(Conversation conversation)
    //{
    //    // await context.Participants.Where(c => c.ConversationId == conversation.Id).ForEachAsync(x =>
    //    // {
    //    //     x.CanAccessConversation = true;
    //    // });

    //    // await context.SaveChangesAsync();
    //}

    //public async Task<bool> HasNewConversation(Conversation conversation)
    //{
    //    // if (await context.Participants.AnyAsync(p => p.ConversationId == conversation.Id && !p.CanAccessConversation))
    //    //     return true;
    //    // else
    //    //     return false;

    //    return true;
    //}
}
