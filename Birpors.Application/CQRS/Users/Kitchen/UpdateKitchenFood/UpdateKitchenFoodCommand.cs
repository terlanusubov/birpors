using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Users.Kitchen.UpdateKitchenFood
{
    public class UpdateKitchenFoodCommandRequest
    {
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> DeletedImages { get; set; }
        public decimal Price { get; set; }
        public bool HasDiscount { get; set; }
        public int DiscountPercentage { get; set; }
        public int CategoryId { get; set; }
        public decimal Kalori { get; set; }

        public UpdateKitchenFoodCommandRequest()
        {
            Images = new List<IFormFile>();
            DeletedImages = new List<string>();
        }

    }
    public class UpdateKitchenFoodCommand : IRequest<ApiResult<int>>
    {
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public UpdateKitchenFoodCommandRequest Model { get; set; }
        public UpdateKitchenFoodCommand(int userId, UpdateKitchenFoodCommandRequest model,int foodId)
        {
            Model = model;
            FoodId = foodId;
            UserId = userId;
        }

        public class UpdateKitchenFoodCommandHandler : IRequestHandler<UpdateKitchenFoodCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public UpdateKitchenFoodCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<int>> Handle(UpdateKitchenFoodCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(c => c.Kitchens).ThenInclude(c=>c.KitchenFoods).FirstOrDefaultAsync(c => c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<int>.Error(
                        new Dictionary<string, string> { { "", "Belə bir istifadeci yoxdur." } },
                        (int)HttpStatusCode.BadRequest,
                        "Prosesin icmalında məntiq xətası baş verdi.");
                }


                if (user.Kitchens.Count == 0)
                {
                    return ApiResult<int>.Error(
                      new Dictionary<string, string> { { "", "Belə bir metbex yoxdur." } },
                      (int)HttpStatusCode.BadRequest,
                      "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var food = user.Kitchens.FirstOrDefault().KitchenFoods.Where(c => c.Id == request.FoodId).FirstOrDefault();
                if (food == null)
                {
                    return ApiResult<int>.Error(
                     new Dictionary<string, string> { { "", "Belə bir mehsul yoxdur." } },
                     (int)HttpStatusCode.BadRequest,
                     "Prosesin icmalında məntiq xətası baş verdi.");
                }


                food.Name = request.Model.Name;
                food.Price = request.Model.Price;
                food.DiscountPercentage = request.Model.DiscountPercentage;
                food.Updated = DateTime.Now;
                food.CategoryId = request.Model.CategoryId;
                food.Ingredients = request.Model.Ingredients;
                food.Kalori = request.Model.Kalori;



                if (request.Model.DeletedImages.Count != 0)
                {
                    foreach (var deleteImage in request.Model.DeletedImages)
                    {
                        var deleteImageName =  deleteImage.Substring(deleteImage.LastIndexOf("/foods/") + 7, deleteImage.Length- (deleteImage.LastIndexOf("/foods/") + 7));
                        var deleteKitchenFood = await _context
                                                        .KitchenFoodPhotos
                                                            .FirstOrDefaultAsync(c => c.Image == deleteImageName);

                        if (deleteKitchenFood != null)
                        {
                            string deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "foods", deleteKitchenFood.Image);

                            bool hasFileInFolder = System.IO.File.Exists(deleteFilePath);

                            if (hasFileInFolder)
                            {
                                System.IO.File.Delete(deleteFilePath);
                            }


                            _context.KitchenFoodPhotos.Remove(deleteKitchenFood);
                            await _context.SaveChanges(cancellationToken);
                        }

                    }
                }


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


                await _context.SaveChanges(cancellationToken);
                return ApiResult<int>.OK(food.Id);
            }
        }
    }
}
