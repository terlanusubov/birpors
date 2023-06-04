using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class Category
    {
        public Category()
        {
            KitchenFoods = new HashSet<KitchenFood>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<KitchenFood> KitchenFoods { get; set; }
    }
}
