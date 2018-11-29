using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyntinelBot
{
    /// <summary>
    /// Specifies if a notification is to be sent for the mentions.
    /// </summary>
    public class NotificationInfo
    {
        /// <summary>
        /// Gets or sets true if notification is to be sent to the user, false
        /// otherwise.
        /// </summary>
        [JsonProperty(PropertyName = "alert")]
        public bool? Alert {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the NotificationInfo class.
        /// </summary>
        public NotificationInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NotificationInfo class.
        /// </summary>
        /// <param name="alert">true if notification is to be sent to the
        /// user, false otherwise.</param>
        public NotificationInfo(bool? alert = null)
        {
            this.Alert = alert;
        }
    }
}
