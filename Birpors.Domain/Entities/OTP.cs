using System;
namespace Birpors.Domain.Entities
{
    public class OTP
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string OneTimePassword { get; set; }
        public string IpAddress { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsRegister { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}

