using Birpors.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Conversation> Conversations { get; set; }
        DbSet<Participant> Participants { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<OTP> OTPs { get; set; }
        DbSet<AppLog> AppLogs { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Kitchen> Kitchens { get; set; }
        DbSet<KitchenFood> KitchenFoods { get; set; }
        DbSet<KitchenFoodStatus> KitchenFoodStatuses { get; set; }
        DbSet<KitchenPhoto> KitchenPhotos { get; set; }
        DbSet<KitchenFoodPhoto> KitchenFoodPhotos { get; set; }
        DbSet<KitchenStatus> KitchenStatuses { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderDetail> OrderDetails { get; set; }
        DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        DbSet<SubscriptionPlanStatus> SubscriptionPlanStatuses { get; set; }
        DbSet<SubscriptionUser> SubscriptionUsers { get; set; }
        DbSet<SubscriptionUserStatus> SubscriptionUserStatuses { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<UserAddrese> UserAddreses { get; set; }
        DbSet<UserCard> UserCards { get; set; }
        DbSet<UserDevice> UserDevices { get; set; }
        DbSet<UserDeviceStatus> UserDeviceStatuses { get; set; }
        DbSet<UserCardStatus> UserCardStatuses { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<UserStatus> UserStatuses { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }

        Task<int> SaveChanges(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollBackTransactionAsync(CancellationToken cancellationToken = default);

    }
}
