using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs.Admin;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Admin.Users.Queries.GetUsers
{
    public class GetUsersRequest
    {
        public int Take { get; set; }

        public int Page { get; set; } = 1;
    }
    public class GetUsersQuery : IRequest<List<UserDto>>
    {

        public GetUsersRequest Model { get; set; }
        public GetUsersQuery(GetUsersRequest model)
        {
            Model = model;
        }

        public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
        {
            private readonly IApplicationDbContext _context;
            public GetUsersQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(c=>c.UserRole)
                                         //.Skip((request.Model.Page - 1) * request.Model.Take)
                                             //.Take(request.Model.Take)
                                              .Select(c => new UserDto
                                              {
                                                  Name = c.Name,
                                                  Surname = c.Surname,
                                                  Email = c.Email,
                                                  Id = c.Id,
                                                  StatusId= c.UserStatusId,
                                                  RoleId = c.UserRoleId,
                                                  RoleName = c.UserRole.Name
                                                  //UserStatus = (UserStatusEnum)Enum.Parse(typeof(UserStatusEnum), c.UserStatusId.ToString())
                                              }).
                                              OrderByDescending(c=>c.Id).ToListAsync();

                return user;
            }
        }
    }
}
