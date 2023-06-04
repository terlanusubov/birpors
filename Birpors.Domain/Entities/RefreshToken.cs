using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Jti { get; set; }
        public DateTime ExpireDate {get;set;}

    }
}