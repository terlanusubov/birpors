using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Account.Commands.Logout
{
    public class LogoutCommand : IRequest<ApiResult<bool>>
    {
        public int? UserId { get; set; }
        public string JTI { get; set; }

        public LogoutCommand(int userId, string jti)
        {
            JTI = jti;
            UserId = userId;
        }

        public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResult<bool>>
        {
            private readonly IApplicationDbContext _context;
            public LogoutCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
            {
                if (request.UserId == null)
                    return ApiResult<bool>.Error(
                                         new Dictionary<string, string> { { "", "Bu telefon nömrəsinə məxsus istifadəçi yoxdur." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");

                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.UserId);

                if (user == null)
                    return ApiResult<bool>.Error(
                                         new Dictionary<string, string> { { "", "Bu telefon nömrəsinə məxsus istifadəçi yoxdur." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");

                var userRefreshToken = await _context.RefreshTokens
                                                    .Where(c => c.UserId == request.UserId &&
                                                                c.Jti == request.JTI )
                                                    .FirstOrDefaultAsync();

                if(userRefreshToken == null)
                    return ApiResult<bool>.Error(
                                         new Dictionary<string, string> { { "", "Refresh token tapılmadı." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");

                _context.RefreshTokens.Remove(userRefreshToken);
                await _context.SaveChanges(cancellationToken);

                return ApiResult<bool>.OK(true);

            }
        }
    }
}
