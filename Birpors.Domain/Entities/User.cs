using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class User
    {
        public User()
        {
            Kitchens = new HashSet<Kitchen>();
            Orders = new HashSet<Order>();
            SubscriptionUsers = new HashSet<SubscriptionUser>();
            UserAddreses = new HashSet<UserAddrese>();
        }

        public int Id { get; set; }
        public string MasterKey { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string FinCode { get; set; }
        public string Phone { get; set; }
        public byte[] Password { get; set; }
        public byte UserStatusId { get; set; }
        public byte UserRoleId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int DefaultPaymentTypeId {get;set;}
        public decimal DeliverPrice { get; set; }
        public decimal DeliverDistance { get; set; }

        public virtual UserRole UserRole { get; set; }
        public virtual UserStatus UserStatus { get; set; }
        public virtual ICollection<Kitchen> Kitchens { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Order> Ordereds { get; set; }
        public virtual ICollection<SubscriptionUser> SubscriptionUsers { get; set; }
        public virtual ICollection<UserAddrese> UserAddreses { get; set; }
        public virtual ICollection<UserDevice> UserDevices { get; set; }
    }
}
