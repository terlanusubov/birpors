using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.Entities
{
    public class Conversation
    {
        public int Id { get; set; }

        public string SecurityStamp { get; set; }

        public string CookName { get; set; }
        public string CookSurname { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsSupport { get; set; } = false;

        public int CookId { get; set; }
        public int ConversationStatusId { get; set; }
        public ICollection<Participant> Participants { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}