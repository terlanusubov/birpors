using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Users.Kitchen.GetKitchenDetail
{
    public class StatisticModel
    {
        public string Monday { get; set; } = "10";
        public string Tuesday { get; set; } = "22";
        public string Wednesday { get; set; } = "10";
        public string Thursday { get; set; } = "10";
        public string Friday { get; set; } = "22";
        public string Saturday { get; set; } = "22";
        public string Sunday { get; set; } = "22";
    }
    public class GetKitchenDetailQueryResponse
    {
        public string CookName { get; set; }
        public string CookSurname { get; set; }
        public int WaitingOrders { get; set; }
        public int FinishedOrders { get; set; }
        public decimal Balance { get; set; }
        public int ComissionPercentage { get; set; }
        public decimal EarnedMoney { get; set; }
        public decimal Rating { get; set; }
        public StatisticModel Statistics { get; set; }
        public GetKitchenDetailQueryResponse()
        {
            Statistics = new StatisticModel();
        }
    }
    public class GetKitchenDetailQuery : IRequest<ApiResult<GetKitchenDetailQueryResponse>>
    {
        public int UserId { get; set; }
        public GetKitchenDetailQuery(int userId)
        {
            UserId = userId;
        }

        public class GetKitchenDetailQueryHandler : IRequestHandler<GetKitchenDetailQuery, ApiResult<GetKitchenDetailQueryResponse>>
        {
            private readonly IApplicationDbContext _context;
            public GetKitchenDetailQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<GetKitchenDetailQueryResponse>> Handle(GetKitchenDetailQuery request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(c => c.Kitchens).FirstOrDefaultAsync(c => c.Id == request.UserId);


                if (user == null)
                {
                    return ApiResult<GetKitchenDetailQueryResponse>.Error(
                                         new Dictionary<string, string> { { "", "Istifadeci tapilmadi." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var response = new GetKitchenDetailQueryResponse();

                var waitingOrders = await _context.Orders.Where(c => c.CookId == request.UserId && c.OrderStatusId == (byte)OrderStatusEnum.WaitingForAccept).ToListAsync();

                response.WaitingOrders = waitingOrders.Count;

                var finishedOrders = await _context.Orders.Where(c => c.CookId == request.UserId && c.OrderStatusId == (byte)OrderStatusEnum.Completed).ToListAsync();

                response.FinishedOrders = finishedOrders.Count;

                decimal balance = user.Kitchens.FirstOrDefault().Balance;
                response.CookName = user.Name;
                response.CookSurname = user.Surname;
                response.Balance = balance;
                response.ComissionPercentage = 20;
                response.EarnedMoney = balance - (balance * 20 / 100);
                response.Rating = user.Kitchens.FirstOrDefault().Rating;

                return ApiResult<GetKitchenDetailQueryResponse>.OK(response);
            }
        }
    }
}
