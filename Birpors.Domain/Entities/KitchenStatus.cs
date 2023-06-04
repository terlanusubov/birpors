using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class KitchenStatus
    {
        public KitchenStatus()
        {
            Kitchens = new HashSet<Kitchen>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Kitchen> Kitchens { get; set; }
    }
}
