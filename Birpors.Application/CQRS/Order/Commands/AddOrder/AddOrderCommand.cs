using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Order.Commands.AddOrder
{
    public class AddOrderCommandRequest
    {
        public int UserAddressId { get; set; }
        public byte PaymentTypeId { get; set; }
        public List<ProductOrderDto> Products { get; set; }
        public int CookId { get; set; }
    }
    public class AddOrderCommand : IRequest<ApiResult<int>>
    {
        public AddOrderCommandRequest Model { get; set; }
        public int UserId { get; set; }
        public AddOrderCommand(AddOrderCommandRequest model, int userId)
        {
            Model = model;
            UserId = userId;
        }
        public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public AddOrderCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<int>> Handle(AddOrderCommand request, CancellationToken cancellationToken)
            {
                if (request.Model.PaymentTypeId == (byte)PaymentTypeEnum.Cart)
                {
                    return ApiResult<int>.Error(
                     new Dictionary<string, string> { { "", "Bu xidmet aktiv deyil." } },
                     (int)HttpStatusCode.BadRequest,
                     "Prosesin icmalında məntiq xətası baş verdi.");
                }


                var user = await _context.Users.Include(c => c.Orders).Include(c => c.UserAddreses).FirstOrDefaultAsync(c => c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<int>.Error(
                      new Dictionary<string, string> { { "", "Belə bir istifadəçi yoxdur." } },
                      (int)HttpStatusCode.BadRequest,
                      "Prosesin icmalında məntiq xətası baş verdi.");
                }

                if (user.Orders.Any(c => c.OrderStatusId != (byte)OrderStatusEnum.Completed && c.OrderStatusId != (byte)OrderStatusEnum.Canceled))
                {
                    return ApiResult<int>.Error(
                                          new Dictionary<string, string> { { "", "Istifadecinin sifarisi var." } },
                                          (int)HttpStatusCode.BadRequest,
                                          "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var userAddress = user.UserAddreses.FirstOrDefault(c => c.Id == request.Model.UserAddressId);
                if (userAddress == null)
                {
                    return ApiResult<int>.Error(
                     new Dictionary<string, string> { { "", "Istifadəçinin belə bir adresi yoxdur." } },
                     (int)HttpStatusCode.BadRequest,
                     "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var cook = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.Model.CookId);
                if (cook.UserStatusId != (byte)UserStatusEnum.Active)
                {
                    return ApiResult<int>.Error(
                    new Dictionary<string, string> { { "", "Bu aspaz sifaris qebul ede bilmez." } },
                    (int)HttpStatusCode.BadRequest,
                    "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var order = new Birpors.Domain.Entities.Order();
                order.UserAddressId = request.Model.UserAddressId;
                order.TransactionId = Guid.NewGuid().ToString();
                order.Updated = DateTime.Now;
                order.Created = DateTime.Now;
                order.OrderDate = DateTime.Now;
                order.PaymentTypeId = request.Model.PaymentTypeId;
                order.OrderStatusId = (byte)OrderStatusEnum.WaitingForAccept;
                order.UserId = user.Id;
                order.CookId = request.Model.CookId;

                await _context.Orders.AddAsync(order);
                await _context.SaveChanges(cancellationToken);

                decimal totalPrice = 0;
                foreach (var orderedProduct in request.Model.Products)
                {
                    var product = await _context.KitchenFoods.Include(c => c.Kitchen).FirstOrDefaultAsync(c => c.Id == orderedProduct.ProductId);
                    if (product == null)
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChanges();

                        return ApiResult<int>.Error(
                          new Dictionary<string, string> { { "", "Məhsul tapılmadı." } },
                          (int)HttpStatusCode.BadRequest,
                          "Prosesin icmalında məntiq xətası baş verdi.");
                    }

                    if (product.Kitchen.UserId != request.Model.CookId)
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChanges();

                        return ApiResult<int>.Error(
                          new Dictionary<string, string> { { "", "Məhsul bu mətbəxə məxsus deyil." } },
                          (int)HttpStatusCode.BadRequest,
                          "Prosesin icmalında məntiq xətası baş verdi.");
                    }

                    decimal price = 0;
                    if (product.DiscountPercentage != null)
                    {
                        price = product.Price - (product.Price * (decimal)product.DiscountPercentage / 100);

                    }
                    else
                    {
                        price = product.Price;
                    }


                    var orderDetail = new OrderDetail();

                    orderDetail.OrderId = order.Id;
                    orderDetail.KitchenFoodId = product.Id;
                    orderDetail.Note = orderedProduct.Note;
                    orderDetail.Price = price;
                    orderDetail.DiscountPercentage = product.DiscountPercentage == null ? 0 : (decimal)product.DiscountPercentage;
                    orderDetail.Count = orderedProduct.Count;
                    orderDetail.Created = DateTime.Now;
                    orderDetail.TotalPrice = price * orderedProduct.Count;
                    orderDetail.Updated = DateTime.Now;

                    totalPrice += orderDetail.TotalPrice;

                    await _context.OrderDetails.AddAsync(orderDetail);
                }

                order.TotalPrice = totalPrice;
                order.DeliverPrice = user.DeliverPrice;

                await _context.SaveChanges(cancellationToken);


                Conversation conversation = new Conversation
                {
                    ConversationStatusId = (int)ConversationStatusEnum.Active,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CookId = (int)order.CookId,
                    CookName = order.Cook.Name,
                    CookSurname = order.Cook.Surname
                };

                await _context.Conversations.AddAsync(conversation);
                await _context.SaveChanges();

                Participant client = new Participant()
                {
                    AppUserId = user.Id,
                    CanAccessConversation = true,
                    ConversationId = conversation.Id,
                    UserRoleId = user.UserRoleId,
                    Name = user.Name,
                    Surname = user.Surname,
                    HasGettedConversation = false
                };

                Participant cookP = new Participant()
                {
                    AppUserId = (int)order.CookId,
                    CanAccessConversation = true,
                    ConversationId = conversation.Id,
                    UserRoleId = order.Cook.UserRoleId,
                    Name = order.Cook.Name,
                    Surname = order.Cook.Surname,
                    HasGettedConversation = false
                };

                await _context.Participants.AddAsync(client);
                await _context.Participants.AddAsync(cookP);

                await _context.SaveChanges();

                return ApiResult<int>.OK(order.Id);
            }
        }
    }
}
