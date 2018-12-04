using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyntinelBot.Models.Slack
{
    public class SelectedOption
    {

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Action
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("selected_options")]
        public IList<SelectedOption> SelectedOptions { get; set; }
    }

    public class Team
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public class Channel
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class User
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class OriginalMessage
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("bot_id")]
        public string BotId { get; set; }

        [JsonProperty("attachments")]
        public IList<object> Attachments { get; set; }
    }

    public class Payload
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("actions")]
        public IList<Action> Actions { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("team")]
        public Team Team { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("action_ts")]
        public string ActionTs { get; set; }

        [JsonProperty("message_ts")]
        public string MessageTs { get; set; }

        [JsonProperty("attachment_id")]
        public string AttachmentId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("is_app_unfurl")]
        public bool IsAppUnfurl { get; set; }

        [JsonProperty("original_message")]
        public OriginalMessage OriginalMessage { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }
    }

    public class SlackMessage
    {

        [JsonProperty("Payload")]
        public Payload Payload { get; set; }

        [JsonProperty("ApiToken")]
        public string ApiToken { get; set; }
    }
}
