using System;
using Birpors.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Birpors.Domain.DTOs
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

