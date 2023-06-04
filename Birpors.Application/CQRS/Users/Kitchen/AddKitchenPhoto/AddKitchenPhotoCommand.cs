using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
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

namespace Birpors.Application.CQRS.Users.Kitchen.AddKitchenPhoto
{
    public class AddKitchenPhotoCommandRequest
    {
        public List<IFormFile> Photos { get; set; }
        public AddKitchenPhotoCommandRequest()
        {
            Photos = new List<IFormFile>();
        }
    }
    public class AddKitchenPhotoCommand : IRequest<ApiResult<int>>
    {
        public AddKitchenPhotoCommandRequest Model { get; set; }
        public int UserId { get; set; }

        public AddKitchenPhotoCommand(AddKitchenPhotoCommandRequest model, int userId)
        {
            Model = model;
            UserId = userId;
        }
        public class AddKitchenPhotoCommandHandler : IRequestHandler<AddKitchenPhotoCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public AddKitchenPhotoCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<int>> Handle(AddKitchenPhotoCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.UserId);

                if (user == null)
                {
                    return ApiResult<int>.Error(
                                            new Dictionary<string, string> { { "", "Istifadəçi tapılmadı" } },
                                            (int)HttpStatusCode.BadRequest,
                                            "Prosesin icmalında məntiq xətası baş verdi.");
                }

                var kitchen = await _context.Kitchens.Include(c => c.KitchenPhotos).FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (kitchen == null)
                {
                    return ApiResult<int>.Error(
                                            new Dictionary<string, string> { { "", "Metbex tapılmadı" } },
                                            (int)HttpStatusCode.BadRequest,
                                            "Prosesin icmalında məntiq xətası baş verdi.");
                }

                if (kitchen.KitchenPhotos.Count != 0)
                {
                    return ApiResult<int>.Error(
                                           new Dictionary<string, string> { { "", "Metbex sekilleri artiq post olunub." } },
                                           (int)HttpStatusCode.BadRequest,
                                           "Prosesin icmalında məntiq xətası baş verdi.");
                }


                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "kitchens");
                List<string> fileNames = new List<string>();
                foreach (var file in request.Model.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + file.FileName;

                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var kitchenPhoto = new KitchenPhoto();

                    kitchenPhoto.Image = fileName;
                    kitchenPhoto.KitchenId = kitchen.Id;
                    kitchenPhoto.IsFood = false;
                    kitchenPhoto.Updated = DateTime.Now;
                    kitchenPhoto.Created = DateTime.Now;

                    await _context.KitchenPhotos.AddAsync(kitchenPhoto);

                    fileNames.Add(fileName);
                }

                await _context.SaveChanges(cancellationToken);

                return ApiResult<int>.OK(kitchen.Id);
            }
        }
    }
}
