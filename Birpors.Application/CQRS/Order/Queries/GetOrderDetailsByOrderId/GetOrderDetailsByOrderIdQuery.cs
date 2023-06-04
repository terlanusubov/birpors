using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Order.Queries.GetOrderDetailsByOrderId
{
    public class GetOrderDetailsByOrderIdQuery : IRequest<ApiResult<List<OrderDetailDto>>>
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }

        public GetOrderDetailsByOrderIdQuery(int userId, int orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }
        public class GetOrderDetailsByOrderIdQueryHandler : IRequestHandler<GetOrderDetailsByOrderIdQuery, ApiResult<List<OrderDetailDto>>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public GetOrderDetailsByOrderIdQueryHandler(IApplicationDbContext context,
                                                        IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }
            public async Task<ApiResult<List<OrderDetailDto>>> Handle(GetOrderDetailsByOrderIdQuery request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders
                                            .Include(c => c.UserAddress)
                                            .Include(c => c.OrderDetails)
                                                .ThenInclude(c => c.KitchenFood)
                                                .ThenInclude(c => c.KitchenFoodPhotos)
                                                .Where(c => c.Id == request.OrderId)
                                                    .FirstOrDefaultAsync();

                if (order == null)
                    return ApiResult<List<OrderDetailDto>>.Error(
                                       new Dictionary<string, string> { { "", "Bele bir sifaris yoxdur." } },
                                       (int)HttpStatusCode.BadRequest,
                                       "Prosesin icmalında məntiq xətası baş verdi.");


                var baseProductImagePath = _configuration["File:Products"];
                var orderDetails = order.OrderDetails.Select(c => new OrderDetailDto
                {
                    OrderId = c.Id,
                    Count = c.Count,
                    DiscountPercentage = c.DiscountPercentage,
                    FoodName = c.KitchenFood.Name,
                    Image = baseProductImagePath + c.KitchenFood.KitchenFoodPhotos.Where(c => c.IsMain).Select(c => c.Image).FirstOrDefault(),
                    Ingredients = c.KitchenFood.Ingredients,
                    ProductId = c.KitchenFoodId,
                    Note = c.Note,
                    Price = c.Price,
                    TotalPrice = c.TotalPrice,
                    UserAddress = order.UserAddress.AddressDescription,
                    UserAddressId = order.UserAddressId
                }).ToList();

                return ApiResult<List<OrderDetailDto>>.OK(orderDetails);

            }
        }
    }
}
