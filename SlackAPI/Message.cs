using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SlackAPI
{
    // See https://api.slack.com/docs/interactive-message-field-guide
    public class Message
    {
        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("bot_id", NullValueHandling = NullValueHandling.Ignore)]
        public string BotId { get; set; }

        // TODO: Add 'comment' field

        [JsonProperty("delete_original", NullValueHandling = NullValueHandling.Ignore)]
        public bool DeleteOriginal { get; set; }

        [JsonProperty("display_as_bot", NullValueHandling = NullValueHandling.Ignore)]
        public bool DisplayAsBot { get; set; }

        [JsonProperty("file", NullValueHandling = NullValueHandling.Ignore)]
        public File File { get; set; }

        // TODO: Add 'icons' field

        [JsonProperty("inviter", NullValueHandling = NullValueHandling.Ignore)]
        public string Inviter { get; set; }

        [JsonProperty("is_intro", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsIntro { get; set; }

        [JsonProperty("last_read", NullValueHandling = NullValueHandling.Ignore)]
        public string LastRead { get; set; } // pattern: ^\\d{10}\\.\\d{6}$ (Timestamp in format 0123456789.012345)

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("old_name", NullValueHandling = NullValueHandling.Ignore)]
        public string OldName { get; set; }

        [JsonProperty("permalink", NullValueHandling = NullValueHandling.Ignore)]
        public string PermaLink { get; set; } // URI

        [JsonProperty("pinned_to", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> PinnedTo { get; set; } // pattern: ^[T][A-Z0-9]{8}$ (Team ID)

        [JsonProperty("purpose", NullValueHandling = NullValueHandling.Ignore)]
        public string Purpose { get; set; } // URI

        [JsonProperty("reactions", NullValueHandling = NullValueHandling.Ignore)]
        public List<Reaction> Reactions { get; set; }

        // TODO: Add 'replies' field

        [JsonProperty("replace_original", NullValueHandling = NullValueHandling.Ignore)]
        public bool ReplaceOriginal { get; set; }

        [JsonProperty("reply_count", NullValueHandling = NullValueHandling.Ignore)]
        public int ReplyCount { get; set; }

        [JsonProperty("response_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseType { get; set; } // 'in_channel' or 'ephemeral'

        [JsonProperty("source_team", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceTeam { get; set; } // pattern	: ^[T][A-Z0-9]{8}$

        [JsonProperty("subscribed", NullValueHandling = NullValueHandling.Ignore)]
        public bool Subscribed { get; set; }

        [JsonProperty("subtype", NullValueHandling = NullValueHandling.Ignore)]
        public string Subtype { get; set; } // e.g. 'bot_message'

        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        public string Team { get; set; } // pattern	: ^[T][A-Z0-9]{8}$

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("thread_ts", NullValueHandling = NullValueHandling.Ignore)]
        public string ThreadTs { get; set; } // pattern: ^\\d{10}\\.\\d{6}$ (Timestamp in format 0123456789.012345)

        [JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
        public string Topic { get; set; }

        [JsonProperty("ts", NullValueHandling = NullValueHandling.Ignore)]
        public string Ts { get; set; }  // pattern: ^\\d{10}\\.\\d{6}$ (Timestamp in format 0123456789.012345)

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; } // pattern	: ^[UW][A-Z0-9]{8}$

        [JsonProperty("user_profile", NullValueHandling = NullValueHandling.Ignore)]
        public UserProfile UserProfile { get; set; }

        [JsonProperty("user_team", NullValueHandling = NullValueHandling.Ignore)]
        public string UserTeam { get; set; } // pattern: ^[T][A-Z0-9]{8}$

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }

        public Message()
        {
            Type = "message";
        }
    }
}
