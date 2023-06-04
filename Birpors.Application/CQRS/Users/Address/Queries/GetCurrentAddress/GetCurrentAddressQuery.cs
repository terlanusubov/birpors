using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Birpors.Application.CQRS.Users.Address.Queries.GetCurrentAddress
{
    public class GetCurrentAddressQuery : IRequest<ApiResult<UserAddressDto>>
    {
        public int UserId { get; set; }

        public GetCurrentAddressQuery(int userId)
        {
            UserId = userId;
        }
        public class GetCurrentAddressQueryHandler : IRequestHandler<GetCurrentAddressQuery, ApiResult<UserAddressDto>>
        {
            private readonly IApplicationDbContext _context;
            public GetCurrentAddressQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<UserAddressDto>> Handle(GetCurrentAddressQuery request, CancellationToken cancellationToken)
            {
                var userAddress = await _context.UserAddreses
                                                        .Where(c => c.UserId == request.UserId)
                                                        .OrderByDescending(c => c.Id)
                                                                .Select(c => new UserAddressDto
                                                                {
                                                                    Address = c.Address,
                                                                    Latitude = c.Latitude,
                                                                    Longitude = c.Longitude,
                                                                    UserAddressId = c.Id,
                                                                    AddressDescription = c.AddressDescription
                                                                }).FirstOrDefaultAsync();


                return ApiResult<UserAddressDto>.OK(userAddress);
            }
        }
    }
}