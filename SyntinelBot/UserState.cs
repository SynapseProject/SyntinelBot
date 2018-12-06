// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder;
using SyntinelBot.Models;

namespace SyntinelBot
{
    /// <summary>
    /// Stores counter state for the conversation.
    /// Stored in <see cref="Microsoft.Bot.Builder.ConversationState"/> and
    /// backed by <see cref="Microsoft.Bot.Builder.MemoryStorage"/>.
    /// </summary>
    public class UserState : IStoreItem
    {
        /// <summary>
        /// Gets or sets the number of turns in the conversation.
        /// </summary>
        /// <value>The number of turns in the conversation.</value>
        public int TurnCount { get; set; } = 0;

        public string Id { get; set; }

        public string BotId { get; set; }

        public string BotName { get; set; }

        public string Name { get; set; }

        public string ServiceUrl { get; set; }

        public string ChannelId { get; set; }

        public List<Job> Jobs { get; set; }

        public List<Notification> Notifications { get; set; }

        public string ConversationId { get; set; }

        public string ETag { get; set; }
    }
}
