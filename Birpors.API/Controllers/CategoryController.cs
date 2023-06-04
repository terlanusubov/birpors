using Birpors.API.Filters;
using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Categories.Queries.GetCategories;
using Birpors.Domain.DTOs;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Dizaynın adı "Əsas" ilə başlayan bölmələr (İstifadəçi üçün)
        /// Kateqoriyaların siyahısını almaq üçün apiyə GET sorğu göndərilir. Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<List<CategoryDto>>>> Get()
        => await _mediator.Send(new GetCategoriesQuery());
    }
}
