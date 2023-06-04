using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Birpors.Application.CQRS.Products.Queries.GetProductById.GetProducts
{
    public class GetProductsQueryResponse
    {
        public List<ProductDto> Products { get; set; }
        public int TotalPage { get; set; }
    }
    public class GetProductsQuery : IRequest<ApiResult<GetProductsQueryResponse>>
    {
        public int? ProductTypeId { get; set; }
        public int? CategoryId { get; set; }
        public int Page { get; set; }
        public GetProductsQuery(int? productTypeId, int? categoryId, int page)
        {
            ProductTypeId = productTypeId;
            CategoryId = categoryId;
            Page = page;
        }
        public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResult<GetProductsQueryResponse>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public GetProductsQueryHandler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }
            public async Task<ApiResult<GetProductsQueryResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
            {
                var baseLinkForImage = _configuration["File:Products"];

                List<ProductDto> products = null;

                var productsQuery = _context.KitchenFoods.Include(c => c.Kitchen)
                                                        .ThenInclude(c => c.User)
                                                        .Include(c => c.KitchenFoodPhotos)
                                                        .Include(c => c.Kitchen)
                                                        .Where(c => c.Kitchen.KitchenStatusId == (byte)KitchenStatusEnum.Active && c.KitchenFoodStatusId == (byte)KitchenFoodStatusEnum.Active && c.Kitchen.User.UserStatusId == (byte)UserStatusEnum.Active &&
                                                                    (request.CategoryId == null || c.CategoryId == request.CategoryId) &&
                                                                    (request.ProductTypeId == null || (request.ProductTypeId == (byte)ProductTypeEnum.Discount ? c.DiscountPercentage != 0 : true)));


                if (request.ProductTypeId == (byte)ProductTypeEnum.Rated)
                {
                    productsQuery = productsQuery.OrderByDescending(c => c.Kitchen.Rating);

                }


                productsQuery = productsQuery.Skip((request.Page - 1) * Convert.ToInt32(_configuration["List:Products"]))
                    .Take(Convert.ToInt32(_configuration["List:Products"]));


                products = await productsQuery.Select(c => new ProductDto
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
                    CookSurname = c.Kitchen.User.Surname
                }).ToListAsync(cancellationToken);


                int totalPage = await productsQuery.CountAsync() / Convert.ToInt32(_configuration["List:Products"]);

                return ApiResult<GetProductsQueryResponse>.OK(new GetProductsQueryResponse
                {
                    Products = products,
                    TotalPage = totalPage
                });
            }
        }
    }
}