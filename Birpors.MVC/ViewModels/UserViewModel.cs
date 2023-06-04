using Birpors.Domain.DTOs.Admin;
using System.Collections.Generic;

namespace Birpors.MVC.ViewModels
{
    public class UserViewModel
    {
        public List<UserDto> Users { get; set; }
        public List<UserRoleDto> UserRoles { get; set; }
    }
}
