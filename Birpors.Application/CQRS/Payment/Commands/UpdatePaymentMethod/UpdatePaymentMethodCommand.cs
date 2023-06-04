using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Payment.Commands.UpdatePaymentMethod
{
    public class UpdatePaymentMethodCommand : IRequest<ApiResult<int>>
    {
        public int PaymentTypeId { get; set; }
        public int UserId { get; set; }
        public UpdatePaymentMethodCommand(int userId, int paymentTypeId)
        {
            UserId = userId;
            PaymentTypeId = paymentTypeId;
        }
        public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public UpdatePaymentMethodCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<int>> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => request.UserId == c.Id);

                if (user == null)
                    return ApiResult<int>.Error(
                                          new Dictionary<string, string> { { "", "Bu telefon nömrəsinə məxsus istifadəçi yoxdur." } },
                                          (int)HttpStatusCode.BadRequest,
                                          "Prosesin icmalında məntiq xətası baş verdi.");
                
                user.DefaultPaymentTypeId = request.PaymentTypeId;
                await _context.SaveChanges(cancellationToken);
                return ApiResult<int>.OK(request.PaymentTypeId);
            }
        }
    }
}