using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
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

namespace Birpors.Application.CQRS.Products.Queries.GetProductSearch
{

    public class GetProductsSearchQueryResponse
    {
        public List<SearchDto> Data { get; set; }
    }

    public class GetProductsSearchQuery : IRequest<ApiResult<GetProductsSearchQueryResponse>>
    {
        public string Search { get; set; }

        public GetProductsSearchQuery(string search)
        {
            Search = search;
        }


        public class GetProductsSearchQueryHandler : IRequestHandler<GetProductsSearchQuery, ApiResult<GetProductsSearchQueryResponse>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;

            public GetProductsSearchQueryHandler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<ApiResult<GetProductsSearchQueryResponse>> Handle(GetProductsSearchQuery request, CancellationToken cancellationToken)
            {
                string search = request.Search.ToLower();
                List<SearchDto> data = new List<SearchDto>();

                var baseLinkForImage = _configuration["File:Products"];

                var categories = await _context.Categories.Where(c => c.Name.ToLower().StartsWith(search) || c.Name.ToLower().Contains(search)).Select(x => new SearchDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchTypeId = (int)SearchTypeEnum.Category

                }).ToListAsync(cancellationToken);

                var product = await _context.KitchenFoods.Where(c => c.Kitchen.KitchenStatusId == (byte)KitchenStatusEnum.Active && c.KitchenFoodStatusId == (byte)KitchenFoodStatusEnum.Active && c.Name.ToLower().StartsWith(search) || c.Name.ToLower().Contains(search) && c.Kitchen.User.UserStatusId == (byte)UserStatusEnum.Active).Include(k => k.KitchenFoodPhotos).Select(x => new SearchDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchTypeId = (int)SearchTypeEnum.Product,
                    Image = baseLinkForImage + x.KitchenFoodPhotos.Where(f => f.IsMain == true).Select(x => x.Image).FirstOrDefault()

                }).ToListAsync(cancellationToken);

                var cook = await _context.Users.Where(c => c.UserRoleId == (byte)UserRoleEnum.Cook && (c.Name.ToLower().StartsWith(search) || search.ToLower().Contains(c.Name.ToLower()) || c.Surname.ToLower().StartsWith(search) || search.ToLower().Contains(c.Surname.ToLower()))
                 && c.Kitchens.FirstOrDefault().KitchenStatusId == (byte)KitchenStatusEnum.Active
                ).Select(c => new SearchDto
                {
                    Id = c.Id,
                    Name = c.Name + " " + c.Surname,
                    SearchTypeId = (int)SearchTypeEnum.Cook,

                }).ToListAsync(cancellationToken);

                if (categories != null && categories.Count > 0)
                    data.AddRange(categories);

                if (product != null && product.Count > 0)
                    data.AddRange(product);

                if (cook != null && cook.Count > 0)
                    data.AddRange(cook);


                return ApiResult<GetProductsSearchQueryResponse>.OK(new GetProductsSearchQueryResponse
                {
                    Data = data
                });
            }
        }
    }
}
