using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class SubscriptionPlan
    {
        public SubscriptionPlan()
        {
            SubscriptionUsers = new HashSet<SubscriptionUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal AmountMonthly { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public byte SubscriptionPlanStatusId { get; set; }

        public virtual SubscriptionPlanStatus SubscriptionPlanStatus { get; set; }
        public virtual ICollection<SubscriptionUser> SubscriptionUsers { get; set; }
    }
}
