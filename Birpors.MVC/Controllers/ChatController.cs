using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Birpors.MVC.Controllers
{
    public class ChatVm
    {
        public Dictionary<int, string> LastMessages { get; set; }
        public List<Participant> Participants { get; set; }
        public string ProfileId { get; set; }
    }

    [Route("canli-destek")]
    [Authorize(Roles = "Admin")]
    public class ChatController : BaseController
    {
        private readonly IApplicationDbContext _context;
        public ChatController(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Support()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            ChatVm vm = new ChatVm()
            {
                Participants = await _context.Conversations
                                                    .Include(p => p.Participants)
                                                    .ThenInclude(c => c.Conversation)
                                                    .ThenInclude(c=>c.Messages)
                                                    .Where(c => c.Participants.Any(p => p.AppUserId == userId && p.CanAccessConversation))

                                                    .Select(c => c.Participants.FirstOrDefault(p => p.AppUserId != userId))
                                                    .ToListAsync(),



            };

            return View(vm);
        }

        //Ajax functions
        [Route("{securityStamp}/mesajlar")]
        public async Task<JsonResult> GetConversationMessages(string securityStamp)
        {

            if (securityStamp == null)
                return Json(new
                {
                    status =400
                });


            Conversation conversation = await _context.Conversations
                                                        .Include(c => c.Messages)
                                                        .Include(c => c.Participants)
                                                        .FirstOrDefaultAsync(c => c.Id == Convert.ToInt32(securityStamp));
            if (conversation == null)
                return Json(new
                {
                    status = 400
                });


            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            if (userId == null || !conversation.Participants.Any(c => c.AppUserId == userId))
                return Json(new
                {
                    status = 400
                });

            return Json(new
            {
                status = (int)HttpStatusCode.OK,
                data = conversation.Messages
                                    .OrderBy(c => c.CreatedDate)
                                    .Select(c => new
                                    {
                                        text = c.Text,
                                        appUserId = c.SenderId,
                                        photo = "account.png"
                                    }),
                loggedUserId = userId
            });

        }

        [Route("{securityStamp}")]
        [HttpPost]
        public async Task<JsonResult> UpdateConversationMessages(string securityStamp)
        {
            if (securityStamp == null)
                return Json(new
                {
                    status = 400
                });


            Conversation conversation = await _context.Conversations
                                                        .Include(c => c.Messages)
                                                        .Include(c => c.Participants)
                                                        .FirstOrDefaultAsync(c => c.Id == Convert.ToInt32(securityStamp));
            if (conversation == null)
                return Json(new
                {
                    status = 400
                });

            await _context.Messages.Where(c => c.ConversationId == conversation.Id).ForEachAsync(c => c.IsViewed = true);
            await _context.SaveChanges();

            return Json(new
            {
                status = 200
            });
        }


        [Route("oxunmamis-mesajlar")]
        public async Task<JsonResult> GetNanViewedMessages()
        {

            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            Conversation conversation = await _context.Conversations
                                                     .Include(c => c.Messages)
                                                     .Include(c => c.Participants)
                                                     .FirstOrDefaultAsync(c => c.CookId == userId);
            if (conversation == null)
                return Json(new
                {
                    status = 400
                });

            var messages = conversation.Messages.Where(c => !c.IsViewed).Select(c => new
            {
                Text = c.Text,
                Name = c.Conversation.Participants.Where(a=>a.AppUserId != userId).Select(a=>a.Name).FirstOrDefault(),
                Surname = c.Conversation.Participants.Where(a=>a.AppUserId != userId).Select(a=>a.Surname).FirstOrDefault(),
                Created = c.CreatedDate
            }).ToList();


            return Json(new
            {
                status = 200,
                data = messages
            });
        }
    }
}

