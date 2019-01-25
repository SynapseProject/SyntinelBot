namespace SyntinelBot
{
    public class User
    {
        public string Alias { get; set; }

        public string BotId { get; set; }

        public string BotName { get; set; }

        public string ChannelId { get; set; } // "msteams" or "slack"

        public string Id { get; set; }

        public bool IsGroupChannel { get; set; } = false;

        public string Name { get; set; }

        public string ServiceUrl { get; set; }

        public string SlackConversationId { get; set; }

        public string TeamId { get; set; }

        public string TenantId { get; set; } // Microsoft Teams only
    }
}