using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace SyntinelBot.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        public bool Acknowledged { get; set; }

        public User ForUser { get; set; }

        public string Action { get; set; }

        public string Target { get; set; }

        public string From { get; set; } // For EC2 Resizing

        public string To { get; set; } // For EC2 Resizing

        public string ConversationId { get; set; } // Triggered by

        public ChannelAccount Initiator { get; set; }

        public DateTime NotificationTime { get; set; }
    }
}
