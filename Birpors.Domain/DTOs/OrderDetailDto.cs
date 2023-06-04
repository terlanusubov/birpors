using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.DTOs
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string FoodName {get;set;}
        public string Ingredients {get;set;}
        public int UserAddressId {get;set;}
        public string UserAddress {get;set;}
        public decimal Count { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public string Image { get; set; }


    }
}