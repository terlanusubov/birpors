using Birpors.API.Filters;
using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Account.Commands.CreateOTP;
using Birpors.Application.CQRS.Account.Commands.Login;
using Birpors.Application.CQRS.Account.Commands.Logout;
using Birpors.Application.CQRS.Account.Commands.RefreshLogin;
using Birpors.Application.CQRS.Account.Commands.VerifyOTP;
using Birpors.Application.CQRS.Admin.Users.Commands.AdminToken;
using Birpors.Application.CQRS.Payment.Commands.UpdatePaymentMethod;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {

        [HttpPost("get-token")]
        public async Task<ActionResult<ApiResult<string>>> GetToken([FromForm]AdminTokenRequest request)
        {
            return await Mediator.Send(new AdminTokenCommand(request));
        }

        /// <summary>
        /// Dizaynın adı "Giriş" (Aşpaz və İstifadəçi hər iki dizaynda)
        /// Aşpaz və ya adı istifadəçilər sistemə daxil olarkən əgər qeydiyyatdan keçmək istəyirlərsə və ya
        /// login olmaq istəyirlərsə nömrəni 994XXXXXXX şəklində bu api-ə post edir və OTP həmin nömrəyə göndərilir. (Həm aşpaz həm də istifadəçi üçün eynidir)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("otp")]
        public async Task<ActionResult<ApiResult<string>>> CreateOtpForPhoneNumber(CreateOTPCommandRequest request)
           => await Mediator.Send(new CreateOTPCommand(request));

        /// <summary>
        /// Dizaynın adı "Təsdiq" (Aşpaz və İstifadəçi hər iki dizaynda)
        /// Aşpaz və ya istifadəçi OTP -ni təsdiq etmək üçün bu api-ə OTP zamanı gələn kodu, telefon nömrəsini, rolu-nu (aşpaz və ya istifadəçi) , userDeviceId-ni POST edir. 
        /// Əgər istifadəçi əvvəlcədən mövcud idi-sə İstifadəçi login olacaqdır. Yox əgər istifadəçi yenidirsə , əgər adi istifadəçidirsə istifadəçi kimi backenddə register prosesi olacaq , yox aşpazdırsa aşpaza uyğun şəkildə.
        /// Api geriyə User-in secret key-ni qaytaracaq. Həmin key LOGİN zamanı istifadə olunacaq onu biryerdə saxlamaq lazımdır.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("verify-otp")]
        public async Task<ActionResult<ApiResult<string>>> VerifyOTPByPhoneNumber(VerifyOTPCommandRequest request)
            => await Mediator.Send(new VerifyOTPCommand(request));

        /// <summary>
        /// Dizaynda yoxdur.
        /// Login api-si dizaynda görünməyən mobil tərəfdən OTP təsdiq olunduqdan sonra istifadəçinin login olması üçün istifadə olunur. 
        /// Bunun üçün istifadəçinin telefon nömrəsi , userDeviceİd-si və Verify OTPdən qaytarılan SecretKey ilə post etmək lazımdır.
        /// Geriyə qaytarılacaq datalar : İstifadəçinin Token-i (Headerdə Authorization: Bearer token şəklində bütün sorğularda göndərilməlidir.), HasAddedProfile (Şəxsi məlumatlar dizaynı açılsınmı yoxsa yox),HasAddedAddress (Ünvan dizaynı açılsınmı)
        /// DeliverPrice (Əgər aşpazdırsa aşpazın çatdırılma qiyməti gələcəkdə lazım olacaq deyə) 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResult<LoginCommandResponse>>> Login(LoginCommandRequest request)
             => await Mediator.Send(new LoginCommand(request));


        /// <summary>
        /// Dizaynda yoxdur.
        /// Login expire olmuş istifadəçini təkrar ona verilən refresh-token ilə yeniləmək üçün bu api-yə ona verilən RefreshToken post olunur 
        /// Qeyd: Headerdə mütləq şəkildə Authorization : Bearer token göndərilməlidir.
        /// Geriyə Login zamanı qaytarılan datalar qayıdacaqdır.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResult<RefreshTokenResponse>>> RefreshToken(string refreshToken)
        {
            var jtiClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "jti");
            var jti = jtiClaim != null ? jtiClaim.Value : null;
            var response = await Mediator.Send(new RefreshTokenCommand(refreshToken, jti));
            return response;
        }

        /// <summary>
        /// Dizaynda yoxdur.
        /// İstifadəçinin çıxışı üçün bu apiyə sadəcə POST sorğu göndərilir. Headerdə Authorization göndərilməlidir.
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResult<bool>>> Logout()
        {
            var jtiClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "jti");
            var jti = jtiClaim != null ? jtiClaim.Value : null;

            var userClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            var userId = userClaim != null ? Convert.ToInt32(userClaim.Value) : 0;

            var response = await Mediator.Send(new LogoutCommand(userId,jti));
            return response;
        }

    }
}
