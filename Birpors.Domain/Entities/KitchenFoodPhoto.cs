using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.Entities
{
    public class KitchenFoodPhoto
    {
        public int Id {get;set;}
        public int KitchenFoodId {get;set;}
        public string Image {get;set;}
        public bool IsMain {get;set;}
    }
}