using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class KitchenPhoto
    {
        public int Id { get; set; }
        public int KitchenId { get; set; }
        public string Image { get; set; }
        public bool IsFood { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual Kitchen Kitchen { get; set; }
    }
}
