using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.DTOs
{
    public class KitchenDto
    {
        public int KitchenId { get; set; }
        public int KitchenStatusId { get; set; }
        public string KitchenStatusName { get; set; }
        public List<string> KitchenImages { get; set; }
        public List<KitchenDetailDto> KitchenDetails { get; set; }
        public DateTime Created { get; set; }
    }
}
