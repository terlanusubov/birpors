using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class UserCard
    {
        public int Id { get; set; }
        public string CardUid { get; set; }
        public int UserId { get; set; }
        public string CardNumber { get; set; }
        public byte UserCardStatusId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsDefaultPayment { get; set; }

        public virtual UserCardStatus UserCardStatus { get; set; }
    }
}
