using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class Kitchen
    {
        public Kitchen()
        {
            KitchenFoods = new HashSet<KitchenFood>();
            KitchenPhotos = new HashSet<KitchenPhoto>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime Updated { get; set; }
        public decimal Balance { get; set; }
        public byte KitchenStatusId { get; set; }
        public decimal Rating { get; set; }

        public virtual KitchenStatus KitchenStatus { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<KitchenFood> KitchenFoods { get; set; }
        public virtual ICollection<KitchenPhoto> KitchenPhotos { get; set; }
    }
}
