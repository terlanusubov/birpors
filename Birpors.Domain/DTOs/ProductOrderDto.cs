using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.DTOs
{
    public class ProductOrderDto
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public string Note { get; set; }
    }
}
