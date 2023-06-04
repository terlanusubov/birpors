using Birpors.API.Filters;
using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Cook.Queries;
using Birpors.Application.CQRS.Order.Queries.GetCookOrder;
using Birpors.Application.CQRS.Payment.Commands.UpdatePaymentMethod;
using Birpors.Application.CQRS.Users.Address.Commands.CreateUserAddress;
using Birpors.Application.CQRS.Users.Address.Queries.GetCurrentAddress;
using Birpors.Application.CQRS.Users.Kitchen.AddKitchenFood;
using Birpors.Application.CQRS.Users.Kitchen.AddKitchenPhoto;
using Birpors.Application.CQRS.Users.Kitchen.DeleteKitchenFood;
using Birpors.Application.CQRS.Users.Kitchen.GetKitchenDetail;
using Birpors.Application.CQRS.Users.Kitchen.UpdateKitchenFood;
using Birpors.Application.CQRS.Users.Kitchen.UpdateOrderStatus;
using Birpors.Application.CQRS.Users.Profile.Commands;
using Birpors.Domain;
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
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        /// <summary>
        /// Dizaynın adı "Şəxsi məlumatlar" , "Hesab"  (İstifadəçi )  | "Şəxsi məlumatlar" , "Profil" (Aşpaz)
        /// Aşpaz və ya istifadəçi qeydiyyatdan keçərkın və ya hesabını update edərkən şəxsi məlumatlarını dəyişmək üçün Bu APİyə POST soru göndərilir.
        /// Mobil nömrəni dəyişmək çıxarılıb , İstifadəçidirsə əgər Ad soyad və email post olunur, Aşpaz üçün əlavə FinCode da yerləşdirilib.
        /// İstifadəçi üçün FinCode boş göndərilir.Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{userId}")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<int>>> UpdateUser(
            UpdateUserProfileCommandRequest request,
            int userId
        ) => await Mediator.Send(new UpdateUserProfileCommand(request, userId));

        /// <summary>
        /// Dizaynın adı "Ünvan" (İstifadəçi) | "Ünvan" (Aşpaz)
        /// İstifadəçi və ya aşpaz ünvan əlavə edərkən bu APİyə POST sorğu göndərilir.
        /// Address text,AddressDescription,Longitude,Latitude,IsMain (əsas adres kimi qəbul edilsinmi) - Əgər aşpazdırsa əlavə olaraq DeliverDistance (çatdırılma məsafəsi) və DeliverPrice (Çatdırılma qiyməti - dizaynda yoxdur ora bir input əlavə edin çatıdırlma qiymətini yazsın adam)  - Əgər istifadəçidirsə sonuncu 2 datanı göndərmirsiz.
        /// Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("address")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<int>>> AddAddress(CreateUserAddressRequest model)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            return await Mediator.Send(new CreateUserAddressCommand(model, userId));
        }

        /// <summary>
        /// Dizaynın adı "Ünvan" (İstifadəçi və Aşpaz üçün)
        /// İstifadəçinin cari seçilmiş əsas adresinin nə olduğunu bilmək üçün bu APİyə GET sorğu göndərilir. Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <returns></returns>
        [HttpGet("address")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<UserAddressDto>>> GetCurrentAddress()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetCurrentAddressQuery(userId));
        }

        /// <summary>
        /// Dizaynın adı "Profil" (İstifadəçi üçün)
        /// İstifadəçi tərəfdən mətbəxinə baxmaq istədiyiniz aşpazın userİd sini bu APİyə GET sorğu olaraq göndərilməlidir.
        /// Geriyə Aşpazın Mətbəxindəki məhsulların Listi qayıdacaqdır.Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/products")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<List<ProductDto>>>> Products(int userId) =>
            await Mediator.Send(new GetCookProductsQuery(userId));

        /// <summary>
        /// Dizaynın adı "Əsas" (Aşpaz üçün)
        /// Aşpaz tərəfdə "Əsas" adlı dizaynda , aşpaz login və ya qeydiyyatdan keçəndən sonra açılacaq olan
        /// ana səhifə üçün APİdir. Aşpazın ad soyadı,Gözləyən sifariş sayı,Bitirmiş sifariş sayı,Balansı,Komissiya faizi,Qazandığı pul
        /// götürmək üçün APİyə GET sorğu göndərilir. Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <returns></returns>
        [HttpGet("kitchen/details")]
        [Auth(UserRoleEnum.Cook)]
        public async Task<
            ActionResult<ApiResult<GetKitchenDetailQueryResponse>>
        > GetKitchenDetails()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetKitchenDetailQuery(userId));
        }

        /// <summary>
        ///  Aspazin gozleyen ve ya bitmis sifarisleri   Completed = 10,OnWay = 20,Preparing = 30,Canceled = 40,WaitingForAccept = 50
        /// </summary>
        /// <returns></returns>
        [HttpGet("kitchen/orders")]
        [Auth(UserRoleEnum.Cook)]
        public async Task<ActionResult<ApiResult<GetCookOrderQueryResponse>>> GetOrders(
            int orderStatusId
        )
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetCookOrderQuery(userId, orderStatusId));
        }

        /// <summary>
        /// Aspaz terefde sifarisin statusunun update olunmasi (statuslar yuxardaki api de qeyd olunub)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("kitchen/orders")]
        [Auth(UserRoleEnum.Cook)]
        public async Task<
            ActionResult<ApiResult<UpdateOrderStatusCommandResponse>>
        > UpdateOrderStatus(UpdateOrderStatusCommandRequest request)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new UpdateOrderStatusCommand(request, userId));
        }

        /// <summary>
        /// Dizaynın adı "Mətbəx və Ləvazimatları"  (Aşpaz üçün)
        /// Aşpaz ilkin qeydiyyatdan sonra mətbəx ləvazimatlarının şəklini göndərməlidir.
        /// Şəkilləri File type-də Listə yığıb bu APİyə POST sorğu olaraq göndərilməldir.Headerda Authorization göndərilməlidir.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("kitchen/photos")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<int>>> AddKitchenPhotos(
            [FromForm] AddKitchenPhotoCommandRequest request
        )
        {
            request.Photos = HttpContext.Request.Form.Files.ToList();
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new AddKitchenPhotoCommand(request, userId));
        }

        /// <summary>
        /// Dizaynın adı "Əlavə et" (Aşpaz üçün)
        /// Aşpaz üçün yeni məhsul əlavə etmək məqsədi ilə bu APİyə POST soru göndərilməlidir. Swaggerda qeyd olunan datalar ilə post olunmalıdır. Dataların hər biri requireddir (Şəkil göndərilməsədə olar),
        /// bəzi datalar dizayn üzərində yoxdur , lakin məhsulun yaranmasında vacibdir . Məsələn Category seçilməlidir. Bu səbəbdən Category APİsinə GET sorğu göndərib onları list kimi ora yığmaq və seçim zamanı İDsini göndərmək lazımdır
        /// Əlavə olaraq Kalori və s kimi datalar dizaynda qeyd olunmayıb lakin input kimi ora yerləşdirək.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("kitchen/foods")]
        public async Task<ActionResult<ApiResult<int>>> AddKitchenFood(
            [FromForm] AddKitchenFoodCommandRequest request
        )
        {
            request.Images = HttpContext.Request.Form.Files.ToList();
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new AddKitchenFoodCommand(userId, request));
        }

        /// <summary>
        /// Aspaz terefde oz metbexine baxmasi ucun api
        /// </summary>
        /// <returns></returns>
        [HttpGet("kitchen/foods")]
        [Auth(UserRoleEnum.Cook)]
        public async Task<ActionResult<ApiResult<List<ProductDto>>>> GetKitchenFood()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetCookProductsQuery(userId));
        }

        /// <summary>
        /// Aspaz terefde oz metbexideki mehsula baxmasi ucun api
        /// </summary>
        /// <returns></returns>
        [HttpGet("kitchen/foods/{foodId}")]
        [Auth(UserRoleEnum.Cook)]
        public async Task<ActionResult<ApiResult<ProductDto>>> GetKitchenFoodById(int foodId)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
            return await Mediator.Send(new GetCookProductByIdQuery(userId, foodId));
        }

        /// <summary>
        /// Dizaynın adı "Əlavə et" (Aşpaz üçün)
        /// Məhsulun editi
        /// </summary>
        /// <param name="foodId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("kitchen/foods/{foodId}")]
        public async Task<ActionResult<ApiResult<int>>> UpdateKitchenFood(
            int foodId,
            [FromForm] UpdateKitchenFoodCommandRequest request
        )
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;
           
            return await Mediator.Send(new UpdateKitchenFoodCommand(userId, request,foodId));
        }

        /// <summary>
        /// Dizaynın adı "Ödəniş" (İstifadəçi üçün)
        /// İstifadəçi parametrlər bölməsində Default ödəniş növünü dəyişə bilər. Bunun üçün PaymentTypeİd PUT olunmalıdır
        /// Headerda Authorization göndərilməlsidir.
        /// </summary>
        /// <param name="paymentTypedId"></param>
        /// <returns></returns>
        [HttpPut("paymentMethods")]
        [Auth(UserRoleEnum.All)]
        public async Task<ActionResult<ApiResult<int>>> ChangeDefaultPayment(int paymentTypedId)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            return await Mediator.Send(new UpdatePaymentMethodCommand(paymentTypedId, userId));
        }


        /// <summary>
        /// Aspaz terefde mehsulun silinmesi (Delete status almasi)
        /// </summary>
        /// <param name="foodId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Auth(UserRoleEnum.Cook)]
        public async Task<ActionResult<ApiResult<int>>> DeleteKitchenFood(int foodId)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            return await Mediator.Send(new DeleteKitchenFoodCommand(foodId, userId));
        }
    }
}
