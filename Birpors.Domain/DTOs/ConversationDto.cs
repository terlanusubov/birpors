using System;
using Birpors.Domain.Entities;
using System.Collections.Generic;

namespace Birpors.Domain.DTOs
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CookId { get; set; }
        public string CookName { get; set; }
        public string CookSurname { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public int ConversationStatusId { get; set; }
        public int UserId { get; set; }
        public List<MessageDto> Messages { get; set; }
        public bool IsLive { get; set; }
    }
}

