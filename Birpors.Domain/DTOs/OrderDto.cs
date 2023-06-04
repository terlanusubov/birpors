using System;
using Birpors.Domain.Enums;

namespace Birpors.Domain.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStatusId { get; set; }
        public int? Rating { get; set; }
        public string OrderUserName { get; set; }
        public string OrderUserSurname { get; set; }
        public string CookName { get; set; }
        public string CookSurname { get; set; }
        public int? CookUserId { get; set; }
        public string TransactionId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public byte PaymentTypeId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliverPrice { get; set; }
    }
}