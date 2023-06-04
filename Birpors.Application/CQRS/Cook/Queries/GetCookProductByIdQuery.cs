using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Birpors.Domain.Enums;

namespace Birpors.Application.CQRS.Cook.Queries
{

    public class GetCookProductByIdQuery : IRequest<ApiResult<ProductDto>>
    {
        // public GetCookProductsQueryRequest Model { get; set; }
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public GetCookProductByIdQuery(int userId, int foodId)
        {
            UserId = userId;
            FoodId = foodId;
        }
        public class GetCookProductByIdQueryHandler : IRequestHandler<GetCookProductByIdQuery, ApiResult<ProductDto>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public GetCookProductByIdQueryHandler(IApplicationDbContext context,
                                               IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<ApiResult<ProductDto>> Handle(GetCookProductByIdQuery request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.UserRoleId == (byte)UserRoleEnum.Cook && c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<ProductDto>.Error(
                                            new Dictionary<string, string> { { "", "Belə bir istifadəçi yoxdur." } },
                                            (int)HttpStatusCode.BadRequest,
                                             "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var baseLinkForImage = _configuration["File:Products"];

                var userProducts = await _context.KitchenFoods
                                                .Include(c => c.KitchenFoodPhotos)
                                                .Include(c => c.Kitchen)
                                                .Where(c => c.Kitchen.UserId == user.Id && c.Id == request.FoodId && c.KitchenFoodStatusId != (byte)KitchenFoodStatusEnum.Deleted)
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
                                                           }).FirstOrDefaultAsync(cancellationToken);

                return ApiResult<ProductDto>.OK(userProducts);
            }
        }
    }
}
