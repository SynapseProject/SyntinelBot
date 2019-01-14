using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SlackAPI
{
    public class Reaction
    {
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public int Count;

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name;

        [JsonProperty("users", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Users;
    }
}
