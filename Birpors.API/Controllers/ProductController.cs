using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Birpors.API.Filters;
using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Products.Queries.GetProductById;
using Birpors.Application.CQRS.Products.Queries.GetProductById.GetProducts;
using Birpors.Application.CQRS.Products.Queries.GetProductSearch;
using Birpors.Domain;
using Birpors.Domain.DTOs;
using Birpors.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Birpors.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : BaseController
    {

        /// <summary>
        /// Dizaynın adı "Yemək" (İstifadəçi üçün)
        /// Detalına baxılmaq istədiyi məhsulun İdsini bu APİyə GET sorğu olaraq göndərilməlidir.Geriyə məhsul barədə detallı məlumatlar qayıdır.
        /// Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("{productId}")] 
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<ProductDto>>> GetProduct(int productId)
        => await Mediator.Send(new GetProductByIdQuery(productId));

        /// <summary>
        /// Dizaynın adı "Əsas" ilə başlayan səhifələrdəki məhsullar (İstifadəçi üçün)
        /// Əsas ilə başlayan bölmələrdə məhsulların siyahısını çıxarmaq üçün bu APİyə GET sorğu göndərilir. Endirimli,Populyar və ya Category-ə görə məhsulları almaq üçün
        /// ProductTypeİd və Categoryid dən istifadə olunmalıdır. Categoryİd göndərilməzsə bütün kateqroidən məhsullar qayıdacaq.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<GetProductsQueryResponse>>> GetProducts(int? productTypeId, int? categoryId,int page = 1)
        => await Mediator.Send(new GetProductsQuery(productTypeId, categoryId, page));


        [HttpGet("search/{data}")]
        //[Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<GetProductsSearchQueryResponse>>> Search(string data)
        => await Mediator.Send(new GetProductsSearchQuery(data));

    }
}