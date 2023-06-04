using Birpors.Application.CommonModels;
using Birpors.Application.CQRS.Account.Commands.Login;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Account.Commands.CreateOTP
{
    public class CreateOTPCommandRequest
    {
        public string PhoneNumber { get; set; }
    }
    public class CreateOTPCommand : IRequest<ApiResult<string>>
    {
        public CreateOTPCommandRequest Model { get; set; }

        public CreateOTPCommand(CreateOTPCommandRequest model)
        {
            Model = model;
        }
        public class CreateOTPCommandHandler : IRequestHandler<CreateOTPCommand, ApiResult<string>>
        {
            private readonly IApplicationDbContext _context;
            public CreateOTPCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<string>> Handle(CreateOTPCommand request, CancellationToken cancellationToken)
            {

                var phoneNumber = request.Model.PhoneNumber;

                var otp = await _context.OTPs.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber && c.IsActive && !c.IsConfirmed);
                if (otp != null)
                {
                    otp.IsConfirmed = false;
                    otp.IsActive = false;
                    await _context.SaveChanges();
                }

                string sOTP = String.Empty;

                string sTempChars = String.Empty;

                Random rand = new();

                string[] saAllowedCharacters = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

                string otpForDb = "";
                for (int i = 0; i < 4; i++)
                {
                    int p = rand.Next(0, saAllowedCharacters.Length);

                    sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                    otpForDb += sTempChars;

                }

                otp = new OTP();
                otp.OneTimePassword = otpForDb;
                otp.Created = DateTime.Now;
                otp.ExpireDate = DateTime.Now.AddMinutes(15);
                otp.IsActive = true;
                otp.PhoneNumber = phoneNumber;
                otp.IpAddress = "";
                otp.IsConfirmed = false;
                otp.IsRegister = true;

                //    int rResult = remainder == 0 ? 0 : remainder - 1;
                string messageBody =
                    $"OTP: {otpForDb}";

                using (HttpClient client = new HttpClient())
                {
                    string userName = "2523";
                    string pass = "1prsKH92sm";
                    string numberId = "1561";
                    client.BaseAddress = new Uri("http://213.172.86.6:8080/SmileWS2/webSmpp.jsp");

                    var response = await client.GetAsync(
                        $"?username={userName}&password={pass}&numberId={numberId}&msisdn={phoneNumber}&msgBody={messageBody}"
                    );

                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        return ApiResult<string>.Error(
                                          new Dictionary<string, string> { { "", "OTP gonderile bilmedi." } },
                                          (int)HttpStatusCode.BadRequest,
                                          "Prosesin icmalında məntiq xətası baş verdi.");
                    }

                    otp.MessageId = Guid.NewGuid().ToString();
                    await _context.OTPs.AddAsync(otp);
                    await _context.SaveChanges();
                }

                return ApiResult<string>.OK(request.Model.PhoneNumber);
            }
        }
    }
}
