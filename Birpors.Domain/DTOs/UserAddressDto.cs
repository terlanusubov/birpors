using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.DTOs
{
    public class UserAddressDto
    {
        public int UserAddressId { get; set; }
        public string Address {get;set;}
        public string Longitude {get;set;}
        public string Latitude {get;set;}
        public string AddressDescription { get; set; }

    }
}