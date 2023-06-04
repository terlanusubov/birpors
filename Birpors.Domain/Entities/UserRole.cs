using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class UserRole
    {
        public UserRole()
        {
            Users = new HashSet<User>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
