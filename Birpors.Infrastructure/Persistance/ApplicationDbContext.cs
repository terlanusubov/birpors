using Birpors.Application.Interfaces;
using Birpors.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birpors.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<OTP> OTPs { get; set; }
        public virtual DbSet<AppLog> AppLogs { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Kitchen> Kitchens { get; set; }
        public virtual DbSet<KitchenFood> KitchenFoods { get; set; }
        public virtual DbSet<KitchenFoodStatus> KitchenFoodStatuses { get; set; }
        public virtual DbSet<KitchenPhoto> KitchenPhotos { get; set; }
        public virtual DbSet<KitchenStatus> KitchenStatuses { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public virtual DbSet<SubscriptionPlanStatus> SubscriptionPlanStatuses { get; set; }
        public virtual DbSet<SubscriptionUser> SubscriptionUsers { get; set; }
        public virtual DbSet<SubscriptionUserStatus> SubscriptionUserStatuses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAddrese> UserAddreses { get; set; }
        public virtual DbSet<UserCard> UserCards { get; set; }
        public virtual DbSet<UserCardStatus> UserCardStatuses { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserStatus> UserStatuses { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<UserDeviceStatus> UserDeviceStatuses { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<KitchenFoodPhoto> KitchenFoodPhotos { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=mysql-highresultech-threeleaves.aivencloud.com;port=18311;database=dev-birpors;user=avnadmin;password=AVNS_MHtHC22e4afxP3RIcVY;convert zero datetime=True", ServerVersion.AutoDetect("server=mysql-highresultech-threeleaves.aivencloud.com;port=18311;database=dev-birpors;user=avnadmin;password=AVNS_MHtHC22e4afxP3RIcVY;convert zero datetime=True"));
            }
            base.OnConfiguring(optionsBuilder);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await this.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await this.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollBackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await this.Database.RollbackTransactionAsync(cancellationToken);
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserDeviceStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });


            modelBuilder.Entity<Kitchen>(entity =>
            {
                entity.HasComment("Mətbəxlər");

                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.KitchenStatus)
                    .WithMany(p => p.Kitchens)
                    .HasForeignKey(d => d.KitchenStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kitchens_kitchenstatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Kitchens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kitchens_users");
            });

            modelBuilder.Entity<KitchenFood>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Ingredients).IsRequired();

                entity.Property(e => e.Kalori).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.KitchenFoods)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kitchenfoods_categories");

                entity.HasOne(d => d.KitchenFoodStatus)
                    .WithMany(p => p.KitchenFoods)
                    .HasForeignKey(d => d.KitchenFoodStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kitchenfoods");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenFoods)
                    .HasForeignKey(d => d.KitchenId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kitchenfoods_kitchens");
            });

            modelBuilder.Entity<KitchenFoodStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<KitchenPhoto>(entity =>
            {
                entity.Property(e => e.Id)
                .UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Image).IsRequired();

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.Kitchen)
                    .WithMany(p => p.KitchenPhotos)
                    .HasForeignKey(d => d.KitchenId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_kitchenphotos_kitchens");
            });

            modelBuilder.Entity<KitchenStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasComment("Gözləmədə , təsdiq edilib, sifariş qəbul etmir");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.Id, "Unq_Orders_Id")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DeliverDate).HasColumnType("datetime");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.UserAddress)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserAddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_orders_useraddreses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_orders_users");

                entity.HasOne(d => d.Cook)
                   .WithMany(p => p.Ordereds)
                   .HasForeignKey(d => d.CookId)
                   .HasConstraintName("FK_Orders_Users_Cook");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Count).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Note).IsRequired();

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.KitchenFood)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.KitchenFoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_orderdetails_kitchenfoods");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_orderdetails_orders");
            });

            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.HasComment("Abone olmaq üçün təkliflər (planlar)");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AmountMonthly).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.SubscriptionPlanStatus)
                    .WithMany(p => p.SubscriptionPlans)
                    .HasForeignKey(d => d.SubscriptionPlanStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subscriptionplans");
            });

            modelBuilder.Entity<SubscriptionPlanStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<SubscriptionUser>(entity =>
            {
                entity.HasComment("Abone olmuş Aşpazlar");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CommitmentType).HasComment("Ödənişi nağd yoxsa nağdsız edib.");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.SubscribtionTrialEndDate)
                    .HasColumnType("datetime")
                    .HasComment("Aboneliyin Trial başlama tarixi");

                entity.Property(e => e.SubscriptionEndDate)
                    .HasColumnType("datetime")
                    .HasComment("Aboneliyin bitme tarixi");

                entity.Property(e => e.SubscriptionPlanId).HasComment("Abone üsülü (Free trial , Aylıq filan qədər və s)");

                entity.Property(e => e.SubscriptionStartDate)
                    .HasColumnType("datetime")
                    .HasComment("Aboneliyin başlanma tarixi");

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.SubscriptionPlan)
                    .WithMany(p => p.SubscriptionUsers)
                    .HasForeignKey(d => d.SubscriptionPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subscriptionusers");

                entity.HasOne(d => d.SubscriptionUserStatus)
                    .WithMany(p => p.SubscriptionUsers)
                    .HasForeignKey(d => d.SubscriptionUserStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subscriptionuserss");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SubscriptionUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subscriptionusers_users");
            });

            modelBuilder.Entity<SubscriptionUserStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {


                entity.HasComment("Istifadəçilər");

                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(50);

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.Property(e => e.UserRoleId).HasComment("Aşpaz , İstifadəçi");

                entity.HasOne(d => d.UserRole)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_userroles");

                entity.HasOne(d => d.UserStatus)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_userstatuses");
            });

            modelBuilder.Entity<UserAddrese>(entity =>
            {
                entity.HasComment("Istifadəçilərin ünvanları");

                entity.Property(e => e.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasComment("Hansı istifadəçnin ünvanıdır");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAddreses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_useraddreses_users");
            });

            modelBuilder.Entity<UserCard>(entity =>
            {
                entity.HasComment("Istifadəçilərin kartları (kartlar Payriffdən gələn unique data əsasında saxlanılacaqdır)");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CardNumber)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.CardUid)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("CardUID")
                    .HasComment("Payriff tərəfindən verilən unique ID");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.HasOne(d => d.UserCardStatus)
                    .WithMany(p => p.UserCards)
                    .HasForeignKey(d => d.UserCardStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_usercards_usercardstatuses");
            });

            modelBuilder.Entity<UserCardStatus>(entity =>
            {
                entity.HasComment("Istifadəçi kartları üçün statuslar");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasComment("Rollar");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasComment("Istifadəçiyə aid statuslar");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            base.OnModelCreating(modelBuilder);

        }

    }
}
