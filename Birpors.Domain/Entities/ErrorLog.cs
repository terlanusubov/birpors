using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string ErrorText { get; set; }
        public DateTime ErrorOccurDate {get;set;}
        public string IpAddress {get;set;}
    }
}