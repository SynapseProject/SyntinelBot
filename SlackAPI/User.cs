using System;
using Newtonsoft.Json;

namespace SlackAPI
{
    public class User
    {
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [JsonProperty("deleted", NullValueHandling = NullValueHandling.Ignore)]
        public bool Deleted { get; set; }

        [JsonProperty("has_2fa", NullValueHandling = NullValueHandling.Ignore)]
        public bool Has2Fa { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("is_admin", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsAdmin { get; set; }

        [JsonProperty("is_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsBot { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsOwner { get; set; }

        [JsonProperty("is_primary_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPrimaryOwner { get; set; }

        [JsonProperty("is_restricted", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsRestricted { get; set; }

        [JsonProperty("is_ultra_restricted", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsUltraRestricted { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("presence", NullValueHandling = NullValueHandling.Ignore)]
        public string Presence { get; set; }

        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public UserProfile Profile { get; set; }

        [JsonProperty("real_name", NullValueHandling = NullValueHandling.Ignore)]
        public string RealName { get; set; }

        [JsonProperty("team_id", NullValueHandling = NullValueHandling.Ignore)]
        public string TeamId { get; set; } // Pattern: ^[T][A-Z0-9]{8}$

        [JsonProperty("tz", NullValueHandling = NullValueHandling.Ignore)]
        public string Tz { get; set; }

        [JsonProperty("tz_label", NullValueHandling = NullValueHandling.Ignore)]
        public string TzLabel { get; set; }

        [JsonProperty("tz_offset", NullValueHandling = NullValueHandling.Ignore)]
        public int TzOffset { get; set; }

        [JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        public int Updated { get; set; }

        public bool IsSlackBot()
        {
            return Id.Equals("USLACKBOT", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}