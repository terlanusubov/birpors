using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public List<string> Photos { get; set; }
        public decimal Kalori { get; set; }
        public string MainPhoto { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Distance { get; set; }
        public string Ingredients { get; set; }
        public int MaxBuyCount { get; set; } = 10;
        public int RatedPeopleCount { get; set; }
        public byte ProductStatusId { get; set; }
        public decimal DeliverPrice { get; set; }
        public int CookId { get; set; }
        public string CookName { get; set; }
        public string CookSurname { get; set; }
    }
}