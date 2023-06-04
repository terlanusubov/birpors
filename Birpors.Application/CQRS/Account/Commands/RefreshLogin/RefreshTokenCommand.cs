using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Birpors.Application.CQRS.Account.Commands.RefreshLogin
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
        public int UserId { get; set; }
        public DateTime JwtTokenExpireDate { get; set; }

    }
    public class RefreshTokenCommand : IRequest<ApiResult<RefreshTokenResponse>>
    {
        public string RefreshToken { get; set; }
        public string Jti { get; set; }
        public RefreshTokenCommand(string refreshToken, string jti)
        {
            RefreshToken = refreshToken;
            Jti = jti;
        }
        public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResult<RefreshTokenResponse>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public RefreshTokenCommandHandler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }
            public async Task<ApiResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            {
                var refreshTokenModel = await _context.RefreshTokens.FirstOrDefaultAsync(c => c.Jti == request.Jti);

                if (refreshTokenModel == null)
                {
                    return ApiResult<RefreshTokenResponse>.Error(
                                            new Dictionary<string, string> { { "", "Refresh token tapılmadı." } },
                                            (int)HttpStatusCode.BadRequest,
                                            "Prosesin icmalında məntiq xətası baş verdi.");
                }

                if (refreshTokenModel.Token != request.RefreshToken)
                {
                    return ApiResult<RefreshTokenResponse>.Error(
                                         new Dictionary<string, string> { { "", "Refresh token doğru deyil." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");

                }


                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == refreshTokenModel.UserId);
                if (refreshTokenModel.ExpireDate > DateTime.Now)
                {
                    return ApiResult<RefreshTokenResponse>.Error(
                                                             new Dictionary<string, string> { { "", "Refresh token expire olmuşdur." } },
                                                             (int)HttpStatusCode.BadRequest,
                                                             "Prosesin icmalında məntiq xətası baş verdi.");
                }


                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:secret"]);
                var jti = Guid.NewGuid().ToString();
                var tokenExpireDate = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["JwtConfig:expirationInMinutes"]));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims: new[] {
                                                      new Claim("jti",jti),
                                                      new Claim("userId",refreshTokenModel.UserId.ToString()),
                                                      new Claim("roleId",user.UserRoleId.ToString())
                    }),
                    Audience = "Birpors",
                    Issuer = "Tarlan Usubov",
                    Expires = tokenExpireDate,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = Guid.NewGuid();

                var rtm = new RefreshToken
                {
                    Jti = jti,
                    Token = refreshToken.ToString(),
                    UserId = refreshTokenModel.UserId,
                    ExpireDate = DateTime.Now.AddYears(1)
                };

                await _context.RefreshTokens.AddAsync(rtm);
                await _context.SaveChanges(cancellationToken);

                _context.RefreshTokens.Remove(refreshTokenModel);
                await _context.SaveChanges(cancellationToken);



                return ApiResult<RefreshTokenResponse>.OK(new RefreshTokenResponse
                {
                    Token = tokenHandler.WriteToken(token),
                    UserId = rtm.UserId,
                    RefreshToken = rtm.Token,
                    RefreshTokenExpireDate = rtm.ExpireDate,
                    JwtTokenExpireDate = tokenExpireDate
                });
            }
        }
    }
}