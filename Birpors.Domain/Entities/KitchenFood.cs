using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class KitchenFood
    {
        public KitchenFood()
        {
            OrderDetails = new HashSet<OrderDetail>();
            KitchenFoodPhotos = new HashSet<KitchenFoodPhoto>();
        }

        public int Id { get; set; }
        public int KitchenId { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public byte KitchenFoodStatusId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public int Rating { get; set; }
        public int RatedPeopleCount { get; set; }
        public decimal Kalori { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Kitchen Kitchen { get; set; }
        public virtual KitchenFoodStatus KitchenFoodStatus { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<KitchenFoodPhoto> KitchenFoodPhotos { get; set; }
    }
}
