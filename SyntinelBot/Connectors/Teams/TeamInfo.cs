using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyntinelBot
{
    /// <summary>
    /// Describes a team
    /// </summary>
    public class TeamInfo
    {
        /// <summary>
        /// Gets or sets unique identifier representing a team
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets name of team.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the TeamInfo class.
        /// </summary>
        public TeamInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TeamInfo class.
        /// </summary>
        /// <param name="id">Unique identifier representing a team</param>
        /// <param name="name">Name of team.</param>
        public TeamInfo(string id = null, string name = null)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
