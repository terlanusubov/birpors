using System;
namespace Birpors.Domain.DTOs.Admin
{
    public class OrderInvoiceDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string FoodName { get; set; }
        public string Ingredients { get; set; }
        public int UserAddressId { get; set; }
        public string UserAddress { get; set; }
        public decimal Count { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public string Image { get; set; }
        public string CookName { get; set; }
        public string CookSurname { get; set; }
        public string OrderUserName { get; set; }
        public string OrderUserSurname { get; set; }
        public int CookUserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}

