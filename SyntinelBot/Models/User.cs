using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyntinelBot
{
    public class User
    {
        public string Alias { get; set; }

        public string Id { get; set; }

        public string BotId { get; set; }

        public string BotName { get; set; }

        public string ChannelId { get; set; }

        public string Name { get; set; }

        public string ServiceUrl { get; set; }

        public string TenantId { get; set; }

        public bool IsChannel { get; set; } = false;

        public string ConversationId { get; set; }
    }
}
