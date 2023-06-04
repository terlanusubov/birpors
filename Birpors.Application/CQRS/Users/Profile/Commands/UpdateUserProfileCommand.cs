using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Users.Profile.Commands
{
    public class UpdateUserProfileCommandRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string FinCode { get; set; }
    }
    public class UpdateUserProfileCommand : IRequest<ApiResult<int>>
    {
        public UpdateUserProfileCommandRequest Model { get; set; }
        public int Id { get; set; }
        public UpdateUserProfileCommand(UpdateUserProfileCommandRequest model, int id)
        {
            Model = model;
            Id = id;
        }
        public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, ApiResult<int>>
        {
            private readonly IApplicationDbContext _context;
            public UpdateUserProfileCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ApiResult<int>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == request.Id);

                if (user == null)
                {
                    return ApiResult<int>.Error(
                                            new Dictionary<string, string> { { "", "Belə bir istifadəçi yoxdur." } },
                                            (int)HttpStatusCode.Conflict,
                                             "Prosesin icmalında məntiq xətası baş verdi.");
                }


               // var appDbContext = context.RequestServices.GetService<IApplicationDbContext>();
               
                user.Name = request.Model.Name;
                user.Surname = request.Model.Surname;
                user.Email = request.Model.Email;
                user.FinCode = request.Model.FinCode;


                var participant = await _context.Participants.Where(c => c.AppUserId == user.Id && String.IsNullOrEmpty(c.Name) && String.IsNullOrEmpty(c.Surname)).FirstOrDefaultAsync();

                if (participant != null) {
                    participant.Name = user.Name;
                    participant.Surname = user.Surname;
                }
                await _context.SaveChanges(cancellationToken);

                return ApiResult<int>.OK(user.Id);
            }
        }
    }
}
