using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class KitchenFoodStatus
    {
        public KitchenFoodStatus()
        {
            KitchenFoods = new HashSet<KitchenFood>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<KitchenFood> KitchenFoods { get; set; }
    }
}
