using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class SubscriptionPlanStatus
    {
        public SubscriptionPlanStatus()
        {
            SubscriptionPlans = new HashSet<SubscriptionPlan>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubscriptionPlan> SubscriptionPlans { get; set; }
    }
}
