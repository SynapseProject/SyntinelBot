using Newtonsoft.Json;

namespace SlackAPI
{
    // See: https://api.slack.com/docs/attachments
    public class Attachment
    {
        [JsonProperty("actions", NullValueHandling = NullValueHandling.Ignore)]
        public AttachmentAction[] Actions;

        [JsonProperty("author_icon", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorIcon;

        [JsonProperty("author_link", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorLink;

        [JsonProperty("author_name", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorName;

        [JsonProperty("callback_id", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackId;

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color;

        [JsonProperty("fallback", NullValueHandling = NullValueHandling.Ignore)]
        public string Fallback;

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public Field[] Fields;

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public string Footer;

        [JsonProperty("footer_icon", NullValueHandling = NullValueHandling.Ignore)]
        public string FooterIcon;

        [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageUrl;

        [JsonProperty("mrkdwn_in", NullValueHandling = NullValueHandling.Ignore)]
        public string[] MrkdwnIn;

        [JsonProperty("pretext", NullValueHandling = NullValueHandling.Ignore)]
        public string Pretext;

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text;

        [JsonProperty("thumb_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ThumbUrl;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title;

        [JsonProperty("title_link", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleLink;
    }
}