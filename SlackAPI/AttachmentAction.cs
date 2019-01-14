using System;

namespace SlackAPI
{
    //See: https://api.slack.com/docs/message-buttons#action_fields
    public class AttachmentAction
    {
        public AttachmentAction(string name, string text)
        {
            this.name = name;
            this.text = text;
        }
        public string name { get; }
        public string text { get; }
        public string style;
        public string type = "button";
        public string value;
        public ActionConfirm confirm;
    }
}
