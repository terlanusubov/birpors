using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Birpors.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public Conversation Conversation { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Guid { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsViewed { get; set; }
    }
}