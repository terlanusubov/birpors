using Birpors.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.DTOs.Admin
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int StatusId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public UserStatusEnum UserStatus { get ;set ; }
        public UserRoleEnum UserRole { get; set; }
    }
}
