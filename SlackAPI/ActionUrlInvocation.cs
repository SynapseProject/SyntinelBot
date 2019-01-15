using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SlackAPI
{
    public class ActionUrlInvocation
    {
        [JsonProperty("Payload", NullValueHandling = NullValueHandling.Ignore)]
        public Payload Payload { get; set; } // 'interactive_message' or a 'dialog_submission'
    }
}
