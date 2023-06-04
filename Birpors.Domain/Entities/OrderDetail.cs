using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int KitchenFoodId { get; set; }
        public decimal Count { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual KitchenFood KitchenFood { get; set; }
        public virtual Order Order { get; set; }
    }
}
