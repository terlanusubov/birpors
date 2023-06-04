using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.DTOs
{
    public class KitchenDetailDto
    {
        public int KitchenDetailId { get; set; }
        public int KitchenId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal KitchenFoodStatusId { get; set; }
        public string KitchenFoodStatusName { get; set; }
        public DateTime Created { get; set; }
        public int Rating { get; set; }
        public decimal Kalori { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int RatedPeopleCount { get; set; }
        public string MainImage { get; set; }
    }
}
