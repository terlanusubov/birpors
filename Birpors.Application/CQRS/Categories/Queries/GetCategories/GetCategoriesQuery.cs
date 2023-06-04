using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery : IRequest<ApiResult<List<CategoryDto>>>
    {

        public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ApiResult<List<CategoryDto>>>
        {
            private readonly IApplicationDbContext _context;
            public GetCategoriesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ApiResult<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _context.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync(cancellationToken);

                return ApiResult<List<CategoryDto>>.OK(categories);
            }
        }
    }
}
