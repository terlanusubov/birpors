using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.Entities
{
    public class Participant
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public Conversation Conversation { get; set; }
        public int ConversationId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool CanAccessConversation { get; set; }
        public int UserRoleId { get; set; }
        public bool HasGettedConversation { get; set; }
    }
}