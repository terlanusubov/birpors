using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Order.Queries.GetCurrentOrder
{

    public class GetAllOrderQuery : IRequest<ApiResult<List<OrderDto>>>
    {
        public int UserId { get; set; }
        public GetAllOrderQuery(int userId)
        {
            UserId = userId;
        }
        public class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, ApiResult<List<OrderDto>>>
        {
            private readonly IApplicationDbContext _context;

            public GetAllOrderQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ApiResult<List<OrderDto>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
            {

                var userOrders = await _context.Orders
                                            .Where(c => c.UserId == request.UserId)
                                            .Include(c=>c.Cook)
                                              //.Include(c=> c.OrderDetails)
                                              //  .ThenInclude(c=> c.KitchenFood)
                                              //      .ThenInclude(c=> c.Kitchen)
                                              //          .ThenInclude(c=> c.User)
                                                            .Select(c => new OrderDto
                                                            {
                                                                CookName = c.Cook.Name,
                                                                CookSurname = c.Cook.Surname,
                                                                CookUserId = c.Cook.Id,
                                                                OrderDate = c.OrderDate,
                                                                OrderStatusId = c.OrderStatusId,
                                                                TransactionId = c.TransactionId,
                                                                DeliveryDate = c.DeliverDate,
                                                                Rating = c.Rating,
                                                                PaymentTypeId = c.PaymentTypeId,
                                                                OrderUserName = c.User.Name,
                                                                OrderUserSurname = c.User.Surname,
                                                                DeliverPrice = c.DeliverPrice,
                                                                TotalPrice = c.TotalPrice,
                                                                Id = c.Id
                                                            }).OrderByDescending(c=>c.OrderDate).ToListAsync();

                    return ApiResult<List<OrderDto>>.OK(userOrders);
            }
        }
    }
}