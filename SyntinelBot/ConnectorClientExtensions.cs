using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SyntinelBot
{
    /// <summary>
    /// Connector client extensions.
    /// </summary>
    public static class ConnectorClientExtensions
    {
        /// <summary>
        /// Creates or gets direct conversation between a bot and user.
        /// </summary>
        /// <param name="conversationClient">Conversation client instance.</param>
        /// <param name="bot">Bot account.</param>
        /// <param name="user">User to create conversation with.</param>
        /// <param name="tenantId">TenantId of the user.</param>
        /// <returns>Conversation creation or get response.</returns>
        public static async Task<ConversationResourceResponse> CreateOrGetDirectConversation(this IConversations conversationClient, ChannelAccount bot, ChannelAccount user, string tenantId)
        {
            var parameters = new ConversationParameters()
            {
                Bot = bot,
                ChannelData = JObject.FromObject(new TeamsChannelData()
                {
                    Tenant = new TenantInfo()
                    {
                        Id = tenantId
                    }
                }, JsonSerializer.Create(new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                })),
                Members = new List<ChannelAccount>()
                {
                    user
                }
            };
            return await conversationClient.CreateConversationAsync(parameters);
        }

        /// <summary>
        /// Gets the teams connector client.
        /// </summary>
        /// <param name="connectorClient">The connector client.</param>
        /// <returns>Teams connector client.</returns>
//        public static TeamsConnectorClient GetTeamsConnectorClient(this IConnectorClient connectorClient)
//        {
//            return TeamsConnectorClient.Initialize(connectorClient);
//        }
    }
}
