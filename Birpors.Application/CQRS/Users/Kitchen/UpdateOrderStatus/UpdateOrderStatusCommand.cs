using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Users.Kitchen.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandResponse
    {
        public int OrderId { get; set; }
    }
    public class UpdateOrderStatusCommandRequest
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
    }
    public class UpdateOrderStatusCommand : IRequest<ApiResult<UpdateOrderStatusCommandResponse>>
    {
        public UpdateOrderStatusCommandRequest Model { get; set; }
        public int UserId { get; set; }

        public UpdateOrderStatusCommand(UpdateOrderStatusCommandRequest model, int userId)
        {
            Model = model;
            UserId = userId;
        }
        public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, ApiResult<UpdateOrderStatusCommandResponse>>
        {
            private readonly IApplicationDbContext _context;
            public UpdateOrderStatusCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<UpdateOrderStatusCommandResponse>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.UserId);
                if (user == null)
                {
                    return ApiResult<UpdateOrderStatusCommandResponse>.Error(
                                            new Dictionary<string, string> { { "", "Istifadeci tapilmadi." } },
                                            (int)HttpStatusCode.BadRequest,
                                            "Prosesin icmalında məntiq xətası baş verdi.");
                }


                var order = await _context.Orders.FirstOrDefaultAsync(c => c.Id == request.Model.OrderId);
                if (order == null)
                {
                    return ApiResult<UpdateOrderStatusCommandResponse>.Error(
                                                              new Dictionary<string, string> { { "", "Istifadeci tapilmadi." } },
                                                              (int)HttpStatusCode.BadRequest,
                                                              "Prosesin icmalında məntiq xətası baş verdi.");
                }


                order.OrderStatusId = (byte)request.Model.OrderStatusId;

                if (request.Model.OrderStatusId == (byte)OrderStatusEnum.Completed)
                {
                    var kitchen = await _context.Kitchens.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (kitchen == null)
                    {
                        return ApiResult<UpdateOrderStatusCommandResponse>.Error(
                                                            new Dictionary<string, string> { { "", "Istifadeci tapilmadi." } },
                                                            (int)HttpStatusCode.BadRequest,
                                                            "Prosesin icmalında məntiq xətası baş verdi.");

                    }


                    kitchen.Balance += order.TotalPrice - order.DeliverPrice;
                    order.DeliverDate = DateTime.Now;

                    var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.CookId == order.CookId && c.Participants.Any(a => a.UserRoleId == (byte)UserRoleEnum.User && a.AppUserId == order.UserId) &&
                      c.ConversationStatusId == (byte)ConversationStatusEnum.Active && c.IsSupport == false);

                    if (conversation != null)
                    {
                        conversation.ConversationStatusId = (byte)ConversationStatusEnum.Finished;

                        var participants = await _context.Participants.Where(c => c.ConversationId == conversation.Id).ToListAsync();

                        participants.ForEach(participant => participant.CanAccessConversation = false);
                    }
                }

                await _context.SaveChanges();
                return ApiResult<UpdateOrderStatusCommandResponse>.OK(new UpdateOrderStatusCommandResponse { OrderId = order.Id });
            }
        }
    }
}