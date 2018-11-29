using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyntinelBot
{
    /// <summary>
    /// Channel data specific to messages received in Microsoft Teams
    /// </summary>
    public class TeamsChannelData
    {
        /// <summary>
        /// Gets or sets information about the channel in which the message
        /// was sent
        /// </summary>
        [JsonProperty(PropertyName = "channel")]
        public ChannelInfo Channel {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets type of event.
        /// </summary>
        [JsonProperty(PropertyName = "eventType")]
        public string EventType {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets notification settings for the message
        /// </summary>
        [JsonProperty(PropertyName = "notification")]
        public NotificationInfo Notification {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets information about the team in which the message was
        /// sent
        /// </summary>
        [JsonProperty(PropertyName = "team")]
        public TeamInfo Team {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets information about the tenant in which the message was
        /// sent
        /// </summary>
        [JsonProperty(PropertyName = "tenant")]
        public TenantInfo Tenant {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the TeamsChannelData class.
        /// </summary>
        public TeamsChannelData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TeamsChannelData class.
        /// </summary>
        /// <param name="channel">Information about the channel in which the
        /// message was sent</param>
        /// <param name="eventType">Type of event.</param>
        /// <param name="team">Information about the team in which the message
        /// was sent</param>
        /// <param name="notification">Notification settings for the
        /// message</param>
        /// <param name="tenant">Information about the tenant in which the
        /// message was sent</param>
        public TeamsChannelData(ChannelInfo channel = null, string eventType = null, TeamInfo team = null, NotificationInfo notification = null, TenantInfo tenant = null)
        {
            this.Channel = channel;
            this.EventType = eventType;
            this.Team = team;
            this.Notification = notification;
            this.Tenant = tenant;
        }
    }
}
