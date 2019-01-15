using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackAPI
{
    // https://api.slack.com/docs/interactive-message-field-guide#action_payload
    public class Payload
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; } // 'interactive_message' or a 'dialog_submission'

        [JsonProperty("actions", NullValueHandling = NullValueHandling.Ignore)]
        public List<Action> Actions { get; set; }

        [JsonProperty("callback_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackId { get; set; }

        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        public Team Team { get; set; }

        [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
        public Channel Channel { get; set; }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public User User { get; set; }

        [JsonProperty("action_ts", NullValueHandling = NullValueHandling.Ignore)]
        public string ActionTs { get; set; }

        [JsonProperty("message_ts", NullValueHandling = NullValueHandling.Ignore)]
        public string MessageTs { get; set; }

        [JsonProperty("attachment_id", NullValueHandling = NullValueHandling.Ignore)]
        public string AttachmentId { get; set; } // A 1-indexed identifier for the specific attachment.

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonProperty("original_message", NullValueHandling = NullValueHandling.Ignore)]
        public Message OriginalMessage { get; set; }

        [JsonProperty("response_url", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ResponseUrl { get; set; }
    }
}