// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using SyntinelBot.Models;

namespace SyntinelBot
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See <see cref="SyntinelBot"/> constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class BotAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotAccessors"/> class.
        /// </summary>
        public BotAccessors(ConversationState conversationState, UserState userState, ServiceState serviceState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            UserState = userState ?? throw new ArgumentNullException(nameof(userState));

            ServiceState = serviceState ?? throw new ArgumentNullException(nameof(serviceState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="UserData"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string UserDataName { get; } = "Bot.UserData";

        public static string JobDataName { get; } = "Bot.JobData";

        public static string NotificationDataName { get; } = "Bot.NotificationData";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for CounterState.
        /// </summary>
        /// <value>
        /// The accessor stores the turn count for the conversation.
        /// </value>
        public IStatePropertyAccessor<UserData> UserDataAccessor { get; set; }

        public IStatePropertyAccessor<Dictionary<string, Job>> JobDataAccessor { get; set; }

        public IStatePropertyAccessor<Dictionary<string, Notification>> NotificationDataAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="ServiceState"/> object for the service state.
        /// </summary>
        /// <value>The <see cref="ServiceState"/> object.</value>
        public ServiceState ServiceState { get; }

        /// <summary>
        /// Gets the <see cref="UserState"/> object for the user state.
        /// </summary>
        /// <value>The <see cref="UserState"/> object.</value>
        public UserState UserState { get; }
    }
}
