using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SlackAPI.Models.Event
{
    public partial class EventUrlInvocation
    {
        [JsonProperty("SlackMessage", NullValueHandling = NullValueHandling.Ignore)]
        public SlackMessage SlackMessage { get; set; }

        [JsonProperty("ApiToken", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiToken { get; set; }
    }
}
