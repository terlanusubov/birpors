using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Users.Kitchen.DeleteKitchenFood
{
    public class DeleteKitchenFoodCommand : IRequest<ApiResult<int>>
    {
        public int FoodId { get; set; }
        public int UserId { get; set; }

        public DeleteKitchenFoodCommand(int foodId, int userId)
        {
            FoodId = foodId;
            UserId = userId;
        }

        public class DeleteKitchenFoodCommandHandler
            : IRequestHandler<DeleteKitchenFoodCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;

            public DeleteKitchenFoodCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ApiResult<int>> Handle(
                DeleteKitchenFoodCommand request,
                CancellationToken cancellationToken
            )
            {
                var user = await _context.Users
                    .Include(c => c.Kitchens)
                    .FirstOrDefaultAsync(c => c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<int>.Error(
                        new Dictionary<string, string> { { "", "Istifadəçi tapılmadı" } },
                        (int)HttpStatusCode.BadRequest,
                        "Prosesin icmalında məntiq xətası baş verdi."
                    );
                }

                if (user.Kitchens.Count == 0)
                {
                    return ApiResult<int>.Error(
                        new Dictionary<string, string> { { "", "Metbex tapilmadi" } },
                        (int)HttpStatusCode.BadRequest,
                        "Prosesin icmalında məntiq xətası baş verdi."
                    );
                }

                var food = await _context.KitchenFoods
                    .Where(c => c.Kitchen.UserId == user.Id && c.Id == request.FoodId)
                    .FirstOrDefaultAsync();


                if(food == null){
                     return ApiResult<int>.Error(
                        new Dictionary<string, string> { { "", "Mehsul tapilmadi" } },
                        (int)HttpStatusCode.BadRequest,
                        "Prosesin icmalında məntiq xətası baş verdi."
                    );
                }

                food.KitchenFoodStatusId = (byte)KitchenFoodStatusEnum.Deleted;

                await _context.SaveChanges();

                return ApiResult<int>.OK(food.Id);
            }
        }
    }
}
