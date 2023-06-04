using System;
using System.Collections.Generic;

#nullable disable

namespace Birpors.Domain.Entities
{
    public partial class UserDevice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserDeviceId { get; set; }
        public int UserDeviceStatusId {get;set;}
        public virtual UserDeviceStatus UserDeviceStatus {get;set;}

    }
}
