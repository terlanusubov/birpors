using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? CookId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string TransactionId { get; set; }
        public byte PaymentTypeId { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserAddressId { get; set; }
        public byte OrderStatusId { get; set; }
        public DateTime? DeliverDate { get; set; }
        public int? Rating { get; set; }
        public string RatingNote { get; set; }
        public decimal DeliverPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual User User { get; set; }
        public virtual User Cook { get; set; }
        public virtual UserAddrese UserAddress { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
