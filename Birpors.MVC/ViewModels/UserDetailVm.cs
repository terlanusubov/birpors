using Birpors.Domain.DTOs;
using Birpors.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.MVC.ViewModels
{
    public class UserDetailVm
    {
        public UserDto UserDetail { get; set; }
        public List<OrderDto> Orders { get; set; }
        public KitchenDto Kitchen { get; set; }
        public List<UserStatus> UserStatuses { get; set; }
        public List<KitchenStatus> KitchenStatuses { get; set; }

        public int OrderCount { get; set; }
        public decimal InCome { get; set; }
        public decimal Comission { get; set; }
        public decimal Balance { get; set; }
    }
}
