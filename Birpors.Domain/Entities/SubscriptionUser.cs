using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class SubscriptionUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public DateTime? SubscribtionTrialEndDate { get; set; }
        public byte CommitmentType { get; set; }
        public byte SubscriptionUserStatusId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
        public virtual SubscriptionUserStatus SubscriptionUserStatus { get; set; }
        public virtual User User { get; set; }
    }
}
