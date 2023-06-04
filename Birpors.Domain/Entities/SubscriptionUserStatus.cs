using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class SubscriptionUserStatus
    {
        public SubscriptionUserStatus()
        {
            SubscriptionUsers = new HashSet<SubscriptionUser>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubscriptionUser> SubscriptionUsers { get; set; }
    }
}
