using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyntinelBot
{
    /// <summary>
    /// A channel info object which decribes the channel.
    /// </summary>
    public class ChannelInfo
    {
        /// <summary>
        /// Gets or sets unique identifier representing a channel
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets name of the channel
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the ChannelInfo class.
        /// </summary>
        public ChannelInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChannelInfo class.
        /// </summary>
        /// <param name="id">Unique identifier representing a channel</param>
        /// <param name="name">Name of the channel</param>
        public ChannelInfo(string id = null, string name = null)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
