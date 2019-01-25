using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SlackAPI.Models.Event
{
    public partial class SlackMessage
    {
        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonProperty("team_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TeamId { get; set; }

        [JsonProperty("api_app_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiAppId { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public Event Event { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("event_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EventId { get; set; }

        [JsonProperty("event_time", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTime { get; set; }

        [JsonProperty("authed_users", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AuthedUsers { get; set; }
    }
}
