using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.DTOs
{
    public class OrderedProductDto
    {
        public UserAddressDto Address { get; set; }
        public int OrderId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public decimal DeliverPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatusId {get;set;}
        public List<OrderDetailDto> Products { get; set; }
    }
}
