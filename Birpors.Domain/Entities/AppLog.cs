using System;
using System.Collections.Generic;
using System.Text;

namespace Birpors.Domain.Entities
{
    public class AppLog
    {
        public int Id { get; set; }
        public string LogText { get; set; }
        public int LogType { get; set; }
        public DateTime Created { get; set; }
    }
}
