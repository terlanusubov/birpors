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

namespace Birpors.Application.CQRS.Order.Queries.GetCurrentOrder
{

    public class GetCurrentOrderQuery : IRequest<ApiResult<OrderDto>>
    {
        public int UserId { get; set; }

        public GetCurrentOrderQuery(int userId)
        {
            UserId = userId;
        }
        public class GetCurrentOrderQueryHandler : IRequestHandler<GetCurrentOrderQuery, ApiResult<OrderDto>>
        {
            private readonly IApplicationDbContext _context;
            public GetCurrentOrderQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ApiResult<OrderDto>> Handle(GetCurrentOrderQuery request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.Where(o => o.UserId == request.UserId &&
                                            (o.OrderStatusId != (int)OrderStatusEnum.Completed || o.OrderStatusId == (int)OrderStatusEnum.Completed && o.Rating == null))
                                                    .OrderByDescending(o => o.Id)
                                                          .Select(c => new OrderDto
                                                          {
                                                            Id = c.Id,
                                                            Rating = c.Rating,
                                                            OrderStatusId = c.OrderStatusId
                                                          }).FirstOrDefaultAsync();
                return ApiResult<OrderDto>.OK(order);
            }
        }
    }
}