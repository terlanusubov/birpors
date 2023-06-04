using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Birpors.Application.CQRS.Cook.Queries
{
    // public class GetCookProductsQueryRequest
    // {
    //     public int UserId { get; set; }
    // }
    public class GetCookProductsQuery : IRequest<ApiResult<List<ProductDto>>>
    {
        // public GetCookProductsQueryRequest Model { get; set; }
        public int UserId { get; set; }
        public GetCookProductsQuery(int userId)
        {
            UserId = userId;
        }
        public class GetCookProductsQueryHandler : IRequestHandler<GetCookProductsQuery, ApiResult<List<ProductDto>>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public GetCookProductsQueryHandler(IApplicationDbContext context,
                                               IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<ApiResult<List<ProductDto>>> Handle(GetCookProductsQuery request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.UserRoleId == (byte)UserRoleEnum.Cook && c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<List<ProductDto>>.Error(
                                            new Dictionary<string, string> { { "", "Belə bir istifadəçi yoxdur." } },
                                            (int)HttpStatusCode.BadRequest,
                                             "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var baseLinkForImage = _configuration["File:Products"];

                var userProducts = await _context.KitchenFoods
                                                .Include(c => c.KitchenFoodPhotos)
                                                .Include(c => c.Kitchen)
                                                .Where(c => c.Kitchen.UserId == user.Id && c.KitchenFoodStatusId != (byte)KitchenFoodStatusEnum.Deleted)
                                                           .Select(c => new ProductDto
                                                           {
                                                               ProductId = c.Id,
                                                               DiscountPercentage = c.DiscountPercentage == null ? 0 : (decimal)c.DiscountPercentage,
                                                               Distance = 0,
                                                               Ingredients = c.Ingredients,
                                                               Kalori = c.Kalori,
                                                               MainPhoto = baseLinkForImage + c.KitchenFoodPhotos.Where(a => a.IsMain).Select(a => a.Image).FirstOrDefault(),
                                                               MaxBuyCount = 0,
                                                               Name = c.Name,
                                                               Photos = c.KitchenFoodPhotos.Select(a => baseLinkForImage + a.Image).ToList(),
                                                               Price = c.Price,
                                                               RatedPeopleCount = c.RatedPeopleCount,
                                                               Rating = c.Rating,
                                                               CookId = c.Kitchen.UserId,
                                                               CookName = c.Kitchen.User.Name,
                                                               CookSurname = c.Kitchen.User.Surname,
                                                               ProductStatusId = c.KitchenFoodStatusId
                                                           }).ToListAsync(cancellationToken);

                return ApiResult<List<ProductDto>>.OK(userProducts);
            }
        }
    }
}