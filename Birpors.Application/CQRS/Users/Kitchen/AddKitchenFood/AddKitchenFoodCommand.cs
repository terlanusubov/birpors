using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Users.Kitchen.AddKitchenFood
{
    public class AddKitchenFoodCommandRequest
    {
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public List<IFormFile> Images { get; set; }
        public decimal Price { get; set; }
        public bool HasDiscount { get; set; }
        public int? DiscountPercentage { get; set; }
        public int CategoryId { get; set; }
        public decimal Kalori { get; set; }
        public AddKitchenFoodCommandRequest()
        {
            Images = new List<IFormFile>();
        }
    }
    public class AddKitchenFoodCommand : IRequest<ApiResult<int>>
    {
        public AddKitchenFoodCommandRequest Model { get; set; }
        public int UserId { get; set; }
        public AddKitchenFoodCommand(int userId, AddKitchenFoodCommandRequest model)
        {
            Model = model;
            UserId = userId;
        }
        public class AddKitchenFoodCommandHandler : IRequestHandler<AddKitchenFoodCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public AddKitchenFoodCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<int>> Handle(AddKitchenFoodCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<int>.Error(
                        new Dictionary<string, string> { { "", "Belə bir istifadeci yoxdur." } },
                        (int)HttpStatusCode.BadRequest,
                        "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var kitchen = await _context.Kitchens.FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (kitchen == null)
                {
                    return ApiResult<int>.Error(
                      new Dictionary<string, string> { { "", "Belə bir metbex yoxdur." } },
                      (int)HttpStatusCode.BadRequest,
                      "Prosesin icmalında məntiq xətası baş verdi.");
                }


                var food = new KitchenFood()
                {
                    KitchenFoodStatusId = (byte)KitchenFoodStatusEnum.Active,
                    CategoryId = request.Model.CategoryId,
                    Created = DateTime.Now,
                    DiscountPercentage = request.Model.DiscountPercentage,
                    Ingredients = request.Model.Ingredients,
                    Kalori = request.Model.Kalori,
                    Name = request.Model.Name,
                    KitchenId = kitchen.Id,
                    RatedPeopleCount = 0,
                    Rating = 5,
                    Updated = DateTime.Now,
                    Price = request.Model.Price
                };

                await _context.KitchenFoods.AddAsync(food);
                await _context.SaveChanges(cancellationToken);


                if (request.Model.Images.Count != 0)
                {
                    foreach (var image in request.Model.Images)
                    {
                        string imageName = Guid.NewGuid().ToString() + image.FileName;
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "foods", imageName);
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var foodImage = new KitchenFoodPhoto()
                        {
                            Image = imageName,
                            IsMain = true,
                            KitchenFoodId = food.Id
                        };

                        await _context.KitchenFoodPhotos.AddAsync(foodImage);
                        await _context.SaveChanges(cancellationToken);

                    }
                }





                return ApiResult<int>.OK(food.Id);
            }
        }
    }
}
