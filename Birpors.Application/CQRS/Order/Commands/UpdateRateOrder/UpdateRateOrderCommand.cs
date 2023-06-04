using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Order.Commands.UpdateRateOrder
{
    public class UpdateRateOrderCommandRequest
    {
        public int Rating { get; set; }
        public string RatingNote { get; set; }
    }

    public class UpdateRateOrderCommand : IRequest<ApiResult<int>>
    {
        public int OrderId { get; set; }

        public UpdateRateOrderCommandRequest _model;
        public UpdateRateOrderCommand(int orderId, UpdateRateOrderCommandRequest model)
        {
            OrderId = orderId;
            _model = model;
        }

        public class UpdateRateOrderCommandHandler : IRequestHandler<UpdateRateOrderCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;

            public UpdateRateOrderCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<int>> Handle(UpdateRateOrderCommand request, CancellationToken cancellationToken)
            {
                var order = await _context.Orders.Where(c => c.Id == request.OrderId).FirstOrDefaultAsync();

                if (order.Rating != null)
                {
                    //todo error qaytar
                }

                order.Rating = request._model.Rating;
                order.RatingNote = request._model.RatingNote;
                await _context.SaveChanges();
                return ApiResult<int>.OK(order.Id);

            }
        }
    }
}