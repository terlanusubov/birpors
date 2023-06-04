using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Birpors.Application.CQRS.Account.Commands.Login;
using Birpors.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Birpors.Application.CQRS.Admin.Users.Commands.AdminToken
{
    public class AdminTokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdminTokenCommand:IRequest<ApiResult<string>>
    {
        private readonly AdminTokenRequest Model;
        public AdminTokenCommand(AdminTokenRequest model)
        {
            Model = model;
        }

        public class AdminTokenCommandHandler : IRequestHandler<AdminTokenCommand,ApiResult<string>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IConfiguration _configuration;
            public AdminTokenCommandHandler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }
            public async Task<ApiResult<string>> Handle(AdminTokenCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Where(c => c.Email == request.Model.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    return ApiResult<string>.Error(
                     new Dictionary<string, string> { { "", "Bu xidmet aktiv deyil." } },
                     (int)HttpStatusCode.BadRequest,
                     "Prosesin icmalında məntiq xətası baş verdi.");
                }


                if (user.UserRoleId != (byte)UserRoleEnum.Admin)
                {
                    return ApiResult<string>.Error(
                     new Dictionary<string, string> { { "", "Bu xidmet aktiv deyil." } },
                     (int)HttpStatusCode.BadRequest,
                     "Prosesin icmalında məntiq xətası baş verdi.");
                }

                using (SHA256 sha256 = SHA256.Create())
                {
                    var buffer = Encoding.UTF8.GetBytes(request.Model.Password);

                    var hash = sha256.ComputeHash(buffer);

                    if (user.Password.SequenceEqual(hash))
                    {
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
                      
                        return ApiResult<string>.OK(tokenHandler.WriteToken(token));
                    }
                }

                return ApiResult<string>.Error(
                    new Dictionary<string, string> { { "", "Bu xidmet aktiv deyil." } },
                    (int)HttpStatusCode.BadRequest,
                    "Prosesin icmalında məntiq xətası baş verdi.");
            }
        }
    }
}
