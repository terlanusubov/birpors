using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Order.Queries.GetOrderDetail
{
    public class GetOrderDetailQuery : IRequest<ApiResult<OrderDetailDto>>
    {
        public int UserId { get; set; }
        public int OrderDetailId { get; set; }

        public GetOrderDetailQuery(int orderDetailId, int userId)
        {
            UserId = userId;
            OrderDetailId = orderDetailId;
        }
        public class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, ApiResult<OrderDetailDto>>
        {
            private readonly IApplicationDbContext _context;
            public GetOrderDetailQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<OrderDetailDto>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
            {
                var orderDetail = await _context.OrderDetails
                                                        .Include(c => c.KitchenFood)
                                                        .Include(c=>c.Order).ThenInclude(c=>c.UserAddress)
                                                        .Where(c => c.Order.UserId == request.UserId)
                                                                .Select(c => new OrderDetailDto
                                                                {
                                                                    Count = c.Count,
                                                                    DiscountPercentage = c.DiscountPercentage,
                                                                    Image = "",
                                                                    ProductId = c.KitchenFoodId,
                                                                    Note = c.Note,
                                                                    OrderId = c.OrderId,
                                                                    Price = c.Price,
                                                                    TotalPrice = c.TotalPrice,
                                                                    FoodName = c.KitchenFood.Name,
                                                                    UserAddress = c.Order.UserAddress.Address
                                                                }).FirstOrDefaultAsync();



                return ApiResult<OrderDetailDto>.OK(orderDetail);

            }
        }
    }
}