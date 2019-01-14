using Newtonsoft.Json;

namespace SlackAPI
{
    public class Field
    {
        [JsonProperty("@short", NullValueHandling = NullValueHandling.Ignore)]
        public bool Short;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title;

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value;
    }
}