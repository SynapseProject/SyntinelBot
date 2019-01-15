using Newtonsoft.Json;

namespace SlackAPI
{
    public class UserProfile
    {
        [JsonProperty("always_active", NullValueHandling = NullValueHandling.Ignore)]
        public bool AlwaysActive { get; set; }

        [JsonProperty("avatar_hash", NullValueHandling = NullValueHandling.Ignore)]
        public string AvatarHash { get; set; }

        [JsonProperty("display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("display_name_normalized", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayNameNormalized { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public object Fields { get; set; }

        [JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("guest_channels", NullValueHandling = NullValueHandling.Ignore)]
        public string GuestChannels { get; set; }

        [JsonProperty("image_192", NullValueHandling = NullValueHandling.Ignore)]
        public string Image192 { get; set; }

        [JsonProperty("image_24", NullValueHandling = NullValueHandling.Ignore)]
        public string Image24 { get; set; }

        [JsonProperty("image_32", NullValueHandling = NullValueHandling.Ignore)]
        public string Image32 { get; set; }

        [JsonProperty("image_48", NullValueHandling = NullValueHandling.Ignore)]
        public string Image48 { get; set; }

        [JsonProperty("image_512", NullValueHandling = NullValueHandling.Ignore)]
        public string Image512 { get; set; }

        [JsonProperty("image_72", NullValueHandling = NullValueHandling.Ignore)]
        public string Image72 { get; set; }

        [JsonProperty("image_original", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageOriginal { get; set; }

        [JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }

        [JsonProperty("real_name", NullValueHandling = NullValueHandling.Ignore)]
        public string RealName { get; set; }

        [JsonProperty("real_name_normalized", NullValueHandling = NullValueHandling.Ignore)]
        public string RealNameNormalized { get; set; }

        [JsonProperty("skype", NullValueHandling = NullValueHandling.Ignore)]
        public string Skype { get; set; }

        [JsonProperty("status_emoji", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusEmoji { get; set; }

        [JsonProperty("status_expiration", NullValueHandling = NullValueHandling.Ignore)]
        public int StatusExpiration { get; set; }

        [JsonProperty("status_text", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusText { get; set; }

        [JsonProperty("status_text_canonical", NullValueHandling = NullValueHandling.Ignore)]
        public string StatusTextCanonical { get; set; }

        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        public string Team { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
    }
}