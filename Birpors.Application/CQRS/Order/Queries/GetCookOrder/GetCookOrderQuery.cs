using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Order.Queries.GetCookOrder
{
    public class GetCookOrderQueryResponse
    {
      public List<OrderedProductDto> Orders { get; set; }
    }
    public class GetCookOrderQuery:IRequest<ApiResult<GetCookOrderQueryResponse>>
    {
        public int OrderStatusId { get; set; }
        public int UserId { get; set; }
        public GetCookOrderQuery(int userId,int orderStatusId)
        {
            UserId = userId;
            OrderStatusId = orderStatusId;
        }
        public class GetCookOrderQueryHandler : IRequestHandler<GetCookOrderQuery, ApiResult<GetCookOrderQueryResponse>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public GetCookOrderQueryHandler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<ApiResult<GetCookOrderQueryResponse>> Handle(GetCookOrderQuery request, CancellationToken cancellationToken)
            {
                var baseLinkForImage = _configuration["File:Products"];


                var orders = await _context.Orders.Include(c=>c.UserAddress)
                                                    .Include(c=>c.User)
                                                      .Include(c=>c.OrderDetails)
                                                        .Where(c=> (request.OrderStatusId == (byte)OrderStatusEnum.WaitingForAccept?
                                                            (c.OrderStatusId != (byte)OrderStatusEnum.Canceled && c.OrderStatusId != (byte)OrderStatusEnum.Completed): 
                                                            c.OrderStatusId == request.OrderStatusId) && c.CookId == request.UserId)
                                                             .Select(c=> new OrderedProductDto
                                                             {
                                                                 UserName = c.User.Name,
                                                                 UserSurname = c.User.Surname,
                                                                 DeliverPrice = c.DeliverPrice,
                                                                 OrderId = c.Id,
                                                                 TotalPrice = c.TotalPrice,
                                                                 OrderStatusId = c.OrderStatusId,
                                                                 Address = new UserAddressDto
                                                                 {
                                                                     Address = c.UserAddress.Address,
                                                                     AddressDescription = c.UserAddress.AddressDescription,
                                                                     Latitude = c.UserAddress.Latitude,
                                                                     Longitude = c.UserAddress.Longitude,
                                                                     UserAddressId = c.UserAddressId
                                                                 },
                                                                 Products = c.OrderDetails.Select(a=>new OrderDetailDto
                                                                 {
                                                                     Count = a.Count,
                                                                     DiscountPercentage = a.DiscountPercentage,
                                                                     Image = baseLinkForImage+a.KitchenFood.KitchenFoodPhotos.Where(b=>b.IsMain).Select(b=>b.Image).FirstOrDefault(),
                                                                     ProductId = a.KitchenFoodId,
                                                                     Note = a.Note,
                                                                     OrderId = a.OrderId,
                                                                     Price = a.Price,
                                                                     TotalPrice = a.TotalPrice,
                                                                     FoodName = a.KitchenFood.Name,
                                                                 }).ToList()
                                                             })
                                                                .ToListAsync();


                return ApiResult<GetCookOrderQueryResponse>.OK(new GetCookOrderQueryResponse
                {
                    Orders = orders
                });
            }
        }
    }
}
