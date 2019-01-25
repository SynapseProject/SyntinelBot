using System;
using Newtonsoft.Json;

namespace SlackAPI.Models.Event
{
    public partial class Event
    {
        [JsonProperty("client_msg_id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ClientMsgId { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }

        [JsonProperty("ts", NullValueHandling = NullValueHandling.Ignore)]
        public string Ts { get; set; }

        [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
        public string Channel { get; set; }

        [JsonProperty("event_ts", NullValueHandling = NullValueHandling.Ignore)]
        public string EventTs { get; set; }

        [JsonProperty("channel_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ChannelType { get; set; }
    }
}
