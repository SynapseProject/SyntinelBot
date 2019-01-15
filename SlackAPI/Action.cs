using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackAPI
{
    //See: https://api.slack.com/docs/message-buttons#action_fields
    public class Action
    {
        public Action(string name, string text)
        {
            Name = name;
            Text = text;
        }

        [JsonProperty("data_source", NullValueHandling = NullValueHandling.Ignore)]
        public string DataSource { get; set; } // 'static', 'users', 'channels', 'conversations', or 'external'

        [JsonProperty("min_query_length", NullValueHandling = NullValueHandling.Ignore)]
        public int MinQueryLength { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<Option> Options { get; set; } // This replaces and supersedes the options array.

        [JsonProperty("option_groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<OptionGroup> OptionGroups { get; set; }

        [JsonProperty("selected_options", NullValueHandling = NullValueHandling.Ignore)]
        public List<Option> SelectedOptions { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("style", NullValueHandling = NullValueHandling.Ignore)]
        public string Style { get; set; } = "default"; // 'default', 'primary' or 'danger'

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; } = "button";

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        [JsonProperty("confirm", NullValueHandling = NullValueHandling.Ignore)]
        public Confirm Confirm { get; set; }
    }
}