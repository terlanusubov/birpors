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

namespace Birpors.Application.CQRS.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<ApiResult<ProductDto>>
    {
        public int ProductId { get; set; }
        public GetProductByIdQuery(int productId)
        {
            ProductId = productId;
        }
        public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResult<ProductDto>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public GetProductByIdQueryHandler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }
            public async Task<ApiResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
            {

                var baseLinkForImage = _configuration["File:Products"];
                var product = await _context.KitchenFoods
                                                     .Where(c => c.Id == request.ProductId && c.KitchenFoodStatusId == (byte)KitchenFoodStatusEnum.Active)
                                                         .Include(c => c.Kitchen)
                                                         .Include(c => c.KitchenFoodPhotos)
                                                         .Select(c => new ProductDto
                                                         {
                                                             ProductId = c.Id,
                                                             Name = c.Name,
                                                             Kalori = c.Kalori,
                                                             Price = c.Price,
                                                             DiscountPercentage = c.DiscountPercentage == null ? 0 : (decimal)c.DiscountPercentage,
                                                             Distance = 0,
                                                             Ingredients = c.Ingredients,
                                                             MainPhoto = baseLinkForImage + c.KitchenFoodPhotos.Where(a => a.IsMain).Select(a => a.Image).FirstOrDefault(),
                                                             Photos = c.KitchenFoodPhotos.Select(a => baseLinkForImage + a.Image).ToList(),
                                                             RatedPeopleCount = c.RatedPeopleCount,
                                                             Rating = c.Rating,
                                                             CookId = c.Kitchen.UserId,
                                                             DeliverPrice = c.Kitchen.User.DeliverPrice,
                                                             CookName = c.Kitchen.User.Name,
                                                             CookSurname = c.Kitchen.User.Surname
                                                         }).FirstOrDefaultAsync();

                if (product == null)
                {
                    return ApiResult<ProductDto>.Error(
                       new Dictionary<string, string> { { "", "Belə bir məhsul yoxdur." } },
                       (int)HttpStatusCode.BadRequest,
                       "Prosesin icmalında məntiq xətası baş verdi.");
                }

                return ApiResult<ProductDto>.OK(product);
            }
        }
    }
}