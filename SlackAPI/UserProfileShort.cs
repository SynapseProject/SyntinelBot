using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SlackAPI
{
    public class UserProfileShort
    {
        [JsonProperty("avatar_hash", NullValueHandling = NullValueHandling.Ignore)]
        public string AvatarHash;

        [JsonProperty("display_name", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName;

        [JsonProperty("first_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName;

        [JsonProperty("image_72", NullValueHandling = NullValueHandling.Ignore)]
        public string Image72;

        [JsonProperty("is_restricted", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsRestricted;

        [JsonProperty("is_ultra_restricted", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsUltraRestricted;

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name;

        [JsonProperty("real_name", NullValueHandling = NullValueHandling.Ignore)]
        public string RealName;

        [JsonProperty("team", NullValueHandling = NullValueHandling.Ignore)]
        public string Team;

        public override string ToString()
        {
            return RealName;
        }
    }
}
