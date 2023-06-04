using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Birpors.Domain.Entities;
using System.Linq;
using Birpors.Domain.Enums;

namespace Birpors.Application.CQRS.Account.Commands.Login
{
    public class LoginCommandRequest
    {
        public string PhoneNumber { get; set; }
        public string UserDeviceId { get; set; }
        public string SecretKey { get; set; }
    }
    public class LoginCommandResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
        public int UserId { get; set; }
        public DateTime JwtTokenExpireDate { get; set; }
        public bool HasAddedProfile { get; set; }
        public bool HasAddedAddress { get; set; }
        public decimal DeliverPrice { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class LoginCommand : IRequest<ApiResult<LoginCommandResponse>>
    {
        public LoginCommandRequest Model { get; set; }
        public LoginCommand(LoginCommandRequest model)
        {
            Model = model;
        }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResult<LoginCommandResponse>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public LoginCommandHandler(IApplicationDbContext context,
                                       IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<ApiResult<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                //Find User by Phone number
                var user = await _context.Users.Include(c => c.UserAddreses).Include(c => c.Kitchens).FirstOrDefaultAsync(c => c.Phone == request.Model.PhoneNumber && c.MasterKey == request.Model.SecretKey);

                if (user == null)
                    return ApiResult<LoginCommandResponse>.Error(
                                            new Dictionary<string, string> { { "", "Bu telefon nömrəsinə məxsus istifadəçi yoxdur." } },
                                            (int)HttpStatusCode.BadRequest,
                                            "Prosesin icmalında məntiq xətası baş verdi.");

                var userDevice = await _context.UserDevices
                                                    .FirstOrDefaultAsync(c =>
                                                    c.UserDeviceId == request.Model.UserDeviceId &&
                                                     c.UserId == user.Id);


                if (userDevice == null)
                    return ApiResult<LoginCommandResponse>.Error(
                                           new Dictionary<string, string> { { "", "Bu telefon nömrəsinə sahib istifadəçinin belə bir cihazı yoxdur." } },
                                           (int)HttpStatusCode.BadRequest,
                                           "Prosesin icmalında məntiq xətası baş verdi.");


                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(request.Model.SecretKey)))
                {
                    string password = String.Concat(user.Id.ToString(), user.MasterKey, "birporssecurepassword");
                    var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                    if (user.Password.SequenceEqual(passwordHash))
                    {
                        //Bütün devicelar deaktiv et
                        await _context.UserDevices.Where(c =>
                                                     c.UserDeviceId == request.Model.UserDeviceId &&
                                                      c.UserId == user.Id).ForEachAsync(c =>
                                                      {
                                                          c.UserDeviceStatusId = (int)UserDeviceStatusEnum.Deactive;
                                                      });

                        //Cari device aktiv et
                        userDevice.UserDeviceStatusId = (int)UserDeviceStatusEnum.Active;

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:secret"]);
                        var jti = Guid.NewGuid().ToString();
                        var tokenExpireDate = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["JwtConfig:expirationInMinutes"]));
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims: new[] {
                                                      new Claim("jti",jti),
                                                      new Claim("userId",user.Id.ToString()),
                                                      new Claim("roleId",user.UserRoleId.ToString())
                         }),
                            Audience = "Birpors",
                            Issuer = "Tarlan Usubov",
                            Expires = tokenExpireDate,
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var refreshToken = Guid.NewGuid();

                        var refreshTokenModel = new RefreshToken
                        {
                            Jti = jti,
                            Token = refreshToken.ToString(),
                            UserId = user.Id,
                            ExpireDate = DateTime.Now.AddYears(1)
                        };

                        await _context.RefreshTokens.AddAsync(refreshTokenModel);
                        await _context.SaveChanges(cancellationToken);

                        return ApiResult<LoginCommandResponse>.OK(new LoginCommandResponse
                        {
                            Token = tokenHandler.WriteToken(token),
                            UserId = user.Id,
                            RefreshToken = refreshTokenModel.Token,
                            RefreshTokenExpireDate = refreshTokenModel.ExpireDate,
                            JwtTokenExpireDate = tokenExpireDate,
                            HasAddedProfile = user.Name != null && user.Surname != null && user.Email != null,
                            HasAddedAddress = user.UserAddreses != null && user.UserAddreses.Count != 0,
                            DeliverPrice = user.DeliverPrice,
                            Name = user.Name,
                            Surname = user.Surname,
                            PhoneNumber = user.Phone
                        });
                    }
                    else
                        return ApiResult<LoginCommandResponse>.Error(
                                          new Dictionary<string, string> { { "", "Istifadəçinin sistemə daxil olunması zamanı xəta baş verdi. Credentialları yoxlayın." } },
                                          (int)HttpStatusCode.BadRequest,
                                          "Prosesin icmalında məntiq xətası baş verdi.");
                }

            }
        }
    }
}