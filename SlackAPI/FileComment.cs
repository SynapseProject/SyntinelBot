using System;
using Newtonsoft.Json;

namespace SlackAPI
{
    public class FileComment
    {
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment;

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id;

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp;

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User;
    }
}