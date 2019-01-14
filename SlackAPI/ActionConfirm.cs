using System;
using System.Collections.Generic;
using System.Text;

namespace SlackAPI
{
    //see: https://api.slack.com/docs/message-buttons#confirmation_fields
    public class ActionConfirm
    {
        public string title;
        public string text;
        public string ok_text;
        public string dismiss_text;
    }
}
