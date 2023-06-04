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

namespace Birpors.Application.CQRS.Users.Address.Commands.CreateUserAddress
{
    public class CreateUserAddressRequest
    {
        public string AddressDescription { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool IsMain { get; set; }
        public decimal DeliverDistance { get; set; }
        public decimal DeliverPrice { get; set; }
    }
    public class CreateUserAddressCommand : IRequest<ApiResult<int>>
    {
        public CreateUserAddressRequest Model { get; set; }
        public int UserId { get; set; }
        public CreateUserAddressCommand(CreateUserAddressRequest model, int userId)
        {
            Model = model;
            UserId = userId;
        }


        public class CreateUserAddressCommandHandler : IRequestHandler<CreateUserAddressCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public CreateUserAddressCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ApiResult<int>> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
            {
                if (request.UserId == 0)
                    return ApiResult<int>.Error(
                                              new Dictionary<string, string> { { "", "Istifadəçi tapılmadı" } },
                                              (int)HttpStatusCode.BadRequest,
                                              "Prosesin icmalında məntiq xətası baş verdi.");

                if (request.Model.DeliverDistance != 0 && request.Model.DeliverPrice != 0)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.UserId);

                    user.DeliverDistance = request.Model.DeliverDistance;
                    user.DeliverPrice = request.Model.DeliverPrice;

                    await _context.SaveChanges(cancellationToken);
                }

                //var userAddress = await _context.UserAddreses.FirstOrDefaultAsync(c => c.Latitude == request.Model.Latitude &&
                //                                                                     c.Longitude == request.Model.Longitude);


                //if (userAddress != null)
                //    return ApiResult<int>.Error(
                //                          new Dictionary<string, string> { { "", "Bu adres artıq əlavə olunub." } },
                //                          (int)HttpStatusCode.BadRequest,
                //                          "Prosesin icmalında məntiq xətası baş verdi.");



                var userAddress = new Domain.Entities.UserAddrese
                {
                    Address = request.Model.Address,
                    AddressDescription = request.Model.AddressDescription,
                    Created = DateTime.Now,
                    IsMainAddress = true,
                    Latitude = request.Model.Latitude,
                    Longitude = request.Model.Longitude,
                    UserId = request.UserId,
                    Updated = DateTime.Now
                };

                await _context.UserAddreses.AddAsync(userAddress);
                await _context.SaveChanges(cancellationToken);

                return ApiResult<int>.OK(userAddress.Id);

            }
        }
    }
}