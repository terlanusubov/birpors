using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class UserCardStatus
    {
        public UserCardStatus()
        {
            UserCards = new HashSet<UserCard>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserCard> UserCards { get; set; }
    }
}
