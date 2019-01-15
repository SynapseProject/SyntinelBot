using Newtonsoft.Json;

namespace SlackAPI
{
    //see: https://api.slack.com/docs/message-buttons#confirmation_fields
    public class Confirm
    {
        [JsonProperty("title", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("ok_text", NullValueHandling = NullValueHandling.Ignore)]
        public string OkText { get; set; }

        [JsonProperty("dismiss_text", NullValueHandling = NullValueHandling.Ignore)]
        public string DismissText { get; set; }
    }
}