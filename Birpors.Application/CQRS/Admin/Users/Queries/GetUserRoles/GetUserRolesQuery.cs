using Birpors.Application.CQRS.Admin.Users.Queries.GetUsers;
using Birpors.Application.Interfaces;
using Birpors.Domain.DTOs.Admin;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Admin.Users.Queries.GetUserRoles
{
    public class GetUserRolesRequest
    {
        public int Take { get; set; }
    }
    public class GetUserRolesQuery : IRequest<List<UserRoleDto>>
    {
        public GetUserRolesRequest Model { get; set; }
        public GetUserRolesQuery(GetUserRolesRequest model)
        {
            Model = model;
        }

        public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<UserRoleDto>>
        {
            private readonly IApplicationDbContext _context;
            public GetUserRolesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<UserRoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
            {
                var userroles = await _context.UserRoles
                                              //.Skip((request.Model.Page - 1) * request.Model.Take)
                                              //.Take(request.Model.Take)
                                              .Select(c => new UserRoleDto
                                              {
                                                  Id = c.Id,
                                                  Name = c.Name

                                                  //UserStatus = (UserStatusEnum)Enum.Parse(typeof(UserStatusEnum), c.UserStatusId.ToString())
                                              })
                                              .ToListAsync();

                return userroles;
            }

            
        }
    }
}
