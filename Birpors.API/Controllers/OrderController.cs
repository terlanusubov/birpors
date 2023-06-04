using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Birpors.API.Filters;
using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Order.Commands.AddOrder;
using Birpors.Application.CQRS.Order.Commands.UpdateRateOrder;
using Birpors.Application.CQRS.Order.Queries.GetCurrentOrder;
using Birpors.Application.CQRS.Order.Queries.GetOrderDetail;
using Birpors.Application.CQRS.Order.Queries.GetOrderDetailsByOrderId;
using Birpors.Domain.DTOs;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Birpors.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : BaseController
    {

        /// <summary>
        /// Dizaynın adı "Sifariş detalları" (İstifadəçi üçün)
        /// Sifariş olunmuş məhsulun detallarına baxmaq üçün apiyə  GET soröu göndərilir. Arqument olaraq həmin sifarişin OrderDetailİd si göndərilməlidir.
        /// Headerda Authozation göndərilməlidir.
        /// </summary>
        /// <param name="orderDetailId"></param>
        /// <returns></returns>
        [HttpGet("{orderDetailId}")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<OrderDetailDto>>> GetOrderDetailById(int orderDetailId)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            return await Mediator.Send(new GetOrderDetailQuery(orderDetailId, userId));
        }

        /// <summary>
        /// Dizaynın adı "Əsas" ilə başlayan bölmələrdəki sifariş Notification-ı (İstifadəçi üçün)
        /// Cari sifariş olunmuş, qəbul gözləyən, bitmiş amma rating verilməmiş ,yolda olan və ya hazırlanan sifariş barədə məlumat üçün bu apiyə GET sorğu göndərilir.
        /// Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <returns></returns>
        [HttpGet("current")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<OrderDto>>> Get()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetCurrentOrderQuery(userId));
        }

        /// <summary>
        /// Dizaynın adı "Sifarişlər" (İstifadəçi üçün)
        /// İstifadəçi etdiyi sifarişlərin tarixçəsinə baxmaq üçün bu APİyə GET sorğu göndərilməlidir. Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<List<OrderDto>>>> GetAllOrders()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            return await Mediator.Send(new GetAllOrderQuery(userId));
        }

        /// <summary>
        /// Dizaynın adı "Sifarişin detalları"
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId}/details")]
        public async Task<ActionResult<ApiResult<List<OrderDetailDto>>>> GetOrderDetailsByOrderId(int orderId)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetOrderDetailsByOrderIdQuery(userId, orderId));
        }

        /// <summary>
        /// Dizaynın adı "Qiymətləndirmə" (İstifadəçi üçün)
        /// Bitmiş sifarişin qiymətləndirilməsi üçün bu APİyə PUT sorğu göndərilməldir. Orderİd query string üzərindən göndərilir, Rating və RatingNote isə bodydən.
        /// Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPut("{orderId}")]
        [Auth(UserRoleEnum.All)]
        public async Task<ApiResult<int>> UpdateRating(UpdateRateOrderCommandRequest request, int orderId)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new UpdateRateOrderCommand(orderId, request));
        }

        /// <summary>
        /// Dizaynın adı "Sifariş" (İstifadəçi üçün)
        /// Yeni sifariş verilməsi üçün bu APİyə POST sorğu göndərilir.
        /// Bodydə İstifadəçinin seçdiyi addressİd si,PaymentTypeİd ödəniş növü,Sifariş etmək istədiyi məhsullar barədə List şəklində (Məhsulun İdsi,Qeyd və Sayı),Sifariş verdiyi aşpazın İdsi POST olunur.
        /// Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Auth(UserRoleEnum.All)]
        public async Task<ApiResult<int>> AddOrder(AddOrderCommandRequest request)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new AddOrderCommand(request, userId));
        }

    }
}