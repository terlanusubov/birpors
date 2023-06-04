using Birpors.Application.CommonModels;
using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Birpors.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.CQRS.Account.Commands.VerifyOTP
{
    public class VerifyOTPCommandRequest
    {
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public byte RoleId { get; set; }
        public string UserDeviceId { get; set; }
    }
    public class VerifyOTPCommand : IRequest<ApiResult<string>>
    {
        public VerifyOTPCommandRequest Model { get; set; }

        public VerifyOTPCommand(VerifyOTPCommandRequest model)
        {
            Model = model;
        }

        public class VerifyOTPCommandHandler : IRequestHandler<VerifyOTPCommand, ApiResult<string>>
        {
            private readonly IApplicationDbContext _context;
            public VerifyOTPCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<ApiResult<string>> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
            {
                var phoneNumber = "994" + request.Model.PhoneNumber.Remove(0, 1);
                var otp = await _context.OTPs.Where(c => c.PhoneNumber == phoneNumber && !c.IsConfirmed && c.IsActive).FirstOrDefaultAsync();

                if (otp == null)
                {
                    return ApiResult<string>.Error(
                                         new Dictionary<string, string> { { "", "OTP vaxti bitmisdir yeniden cehd edin." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");
                }

                if (otp.OneTimePassword != request.Model.Code)
                {
                    return ApiResult<string>.Error(
                                         new Dictionary<string, string> { { "", "OTP yanlisdir." } },
                                         (int)HttpStatusCode.BadRequest,
                                         "Prosesin icmalında məntiq xətası baş verdi.");
                }

                otp.IsConfirmed = true;
                otp.IsActive = false;
                await _context.SaveChanges();

                var user = await _context.Users.Include(c => c.Kitchens).FirstOrDefaultAsync(c => c.Phone == request.Model.PhoneNumber);


                if (user != null)
                {
                    if (user.UserRoleId != request.Model.RoleId)
                    {
                        return ApiResult<string>.Error(
                                          new Dictionary<string, string> { { "", "Siz bu appdən login ola bilmərsiz." } },
                                          (int)HttpStatusCode.BadRequest,
                                          "Prosesin icmalında məntiq xətası baş verdi.");

                    }
                }

                if (user == null)
                {
                    user = new User
                    {
                        UserRoleId = request.Model.RoleId,
                        UserStatusId = request.Model.RoleId == (byte)UserRoleEnum.User ? (byte)UserStatusEnum.Active : (byte)UserStatusEnum.Deactive,
                        Phone = request.Model.PhoneNumber,
                        Password = new byte[10],
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        MasterKey = Guid.NewGuid().ToString(),
                        DeliverPrice = 0
                    };




                    await _context.Users.AddAsync(user);
                    await _context.SaveChanges(cancellationToken);

                    using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(user.MasterKey)))
                    {
                        string password = String.Concat(user.Id.ToString(), user.MasterKey, "birporssecurepassword");
                        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                        user.Password = passwordHash;

                        await _context.SaveChanges(cancellationToken);

                    }


                }

                var userDevice = await _context.UserDevices.FirstOrDefaultAsync(c => c.UserDeviceId == request.Model.UserDeviceId && c.UserId == user.Id);

                if (userDevice == null)
                {
                    userDevice = new UserDevice
                    {
                        UserId = user.Id,
                        UserDeviceId = request.Model.UserDeviceId,
                        UserDeviceStatusId = (int)UserDeviceStatusEnum.Deactive
                    };
                    await _context.UserDevices.AddAsync(userDevice);
                    await _context.SaveChanges(cancellationToken);
                }



                if (user.UserRoleId == (byte)UserRoleEnum.Cook)
                {
                    var kitchen = user.Kitchens.FirstOrDefault();

                    if (kitchen != null)
                    {
                        return ApiResult<string>.OK(user.MasterKey);
                    }

                    kitchen = new Kitchen();
                    kitchen.UserId = user.Id;
                    kitchen.KitchenStatusId = (byte)KitchenStatusEnum.Waiting;
                    kitchen.Created = DateTime.Now;
                    kitchen.Balance = 0;
                    kitchen.Rating = 5;
                    kitchen.Updated = DateTime.Now;

                    await _context.Kitchens.AddAsync(kitchen);
                    await _context.SaveChanges(cancellationToken);


                    var subscriptionPlan = await _context.SubscriptionPlans.FirstOrDefaultAsync(c => c.Name == "Free");

                    var subscriptionUser = await _context.SubscriptionUsers.FirstOrDefaultAsync(c => c.UserId == user.Id);

                    if (subscriptionUser == null)
                    {
                        subscriptionUser = new SubscriptionUser();

                        subscriptionUser.UserId = user.Id;
                        subscriptionUser.Created = DateTime.Now;
                        subscriptionUser.CommitmentType = (byte)CommitmentTypeEnum.Cart;
                        subscriptionUser.SubscriptionPlanId = subscriptionPlan.Id;
                        subscriptionUser.SubscriptionUserStatusId = (byte)SubscriptionUserStatusEnum.Acitve;
                        subscriptionUser.Updated = DateTime.Now;
                        subscriptionUser.SubscriptionStartDate = DateTime.Now;
                        subscriptionUser.SubscriptionEndDate = DateTime.Now.AddMonths(3);
                        subscriptionUser.SubscribtionTrialEndDate = DateTime.Now.AddMonths(3);

                        await _context.SubscriptionUsers.AddAsync(subscriptionUser);
                        await _context.SaveChanges(cancellationToken);
                    }
                }





                var adminUser = await _context.Users.Where(c => c.UserRoleId == (byte)UserRoleEnum.Admin).FirstOrDefaultAsync();


                if (adminUser != null)
                {

                    var conversation = await _context.Conversations.Where(c => c.CookId == adminUser.Id && c.Participants.Any(a => a.AppUserId == user.Id) && c.IsSupport).FirstOrDefaultAsync();

                    if (conversation == null)
                    {
                        conversation = new Conversation
                        {
                            ConversationStatusId = (int)ConversationStatusEnum.Active,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            CookId = (int)adminUser.Id,
                            CookName = "Canlı",
                            CookSurname = "Dəstək",
                            IsSupport = true
                        };

                        await _context.Conversations.AddAsync(conversation);
                        await _context.SaveChanges();

                        Participant client = new Participant()
                        {
                            AppUserId = user.Id,
                            CanAccessConversation = true,
                            ConversationId = conversation.Id,
                            UserRoleId = user.UserRoleId,
                            Name = user.Name,
                            Surname = user.Surname,
                            HasGettedConversation = false
                        };

                        Participant liveHelp = new Participant()
                        {
                            AppUserId = (int)adminUser.Id,
                            CanAccessConversation = true,
                            ConversationId = conversation.Id,
                            UserRoleId = adminUser.UserRoleId,
                            Name = "Canlı",
                            Surname = "Dəstək",
                            HasGettedConversation = false
                        };

                        await _context.Participants.AddAsync(client);
                        await _context.Participants.AddAsync(liveHelp);

                        await _context.SaveChanges();
                    }
                }
                return ApiResult<string>.OK(user.MasterKey);
            }
        }
    }
}
