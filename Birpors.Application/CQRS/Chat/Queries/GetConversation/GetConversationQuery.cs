using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Chat.Queries.GetConversation
{

    public class GetConversationQueryResponse
    {
        public List<ConversationDto> Conversations { get; set; }
    }
    public class GetConversationQuery : IRequest<ApiResult<GetConversationQueryResponse>>
    {
        private readonly int UserId;
        public GetConversationQuery(int userId)
        {
            UserId = userId;
        }

        public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, ApiResult<GetConversationQueryResponse>>
        {
            private readonly IApplicationDbContext _context;
            public GetConversationQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }


            public async Task<ApiResult<GetConversationQueryResponse>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Where(c => c.Id == request.UserId).FirstOrDefaultAsync();


                var conversations = await _context.Conversations
                    .Include(c => c.Participants)
                    .Where(c =>
                    c.Participants.Any(a => a.AppUserId == request.UserId) &&
                    c.ConversationStatusId == (int)ConversationStatusEnum.Active
                    )
                    .OrderByDescending(c=>c.CreatedDate)
                    .Select(c => new ConversationDto
                    {
                        ConversationId = c.Id,
                        ConversationStatusId = c.ConversationStatusId,
                        CookId = c.CookId,
                        CookName = c.CookName,
                        CookSurname = c.CookSurname,
                        UserName = (user.UserRoleId == (byte)UserRoleEnum.Cook) ? c.CookName: (c.Participants.Where(a => a.UserRoleId == (int)UserRoleEnum.User).Select(a => a.Name).FirstOrDefault()),
                        UserSurname = (user.UserRoleId == (byte)UserRoleEnum.Cook) ? c.CookSurname : c.Participants.Where(a => a.UserRoleId == (int)UserRoleEnum.User).Select(a => a.Surname).FirstOrDefault(),
                        CreatedDate = c.CreatedDate,
                        UserId = c.Participants.Where(a => a.UserRoleId == (int)UserRoleEnum.User).Select(a => a.AppUserId).FirstOrDefault(),
                        Messages = c.Messages.Select(a => new MessageDto
                        {
                            SenderId = a.SenderId,
                            ConversationId = a.ConversationId,
                            CreatedDate = a.CreatedDate,
                            MessageId = a.Id,
                            Text = a.Text
                        }).ToList(),
                        IsLive = false
                    })
                    .ToListAsync();

                await _context.Participants.Where(c => c.AppUserId == request.UserId && !c.HasGettedConversation)
                                             .ForEachAsync(c => c.HasGettedConversation = true);

                await _context.SaveChanges();


                return ApiResult<GetConversationQueryResponse>.OK(new GetConversationQueryResponse
                {
                    Conversations = conversations
                });
            }
        }
    }
}

