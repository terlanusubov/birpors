using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class UserAddrese
    {
        public UserAddrese()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; }
        public string AddressDescription { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool IsMainAddress { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
