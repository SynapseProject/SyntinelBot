using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyntinelBot
{
    /// <summary>
    /// Describes a tenant
    /// </summary>
    public class TenantInfo
    {
        /// <summary>
        /// Gets or sets unique identifier representing a tenant
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the TenantInfo class.
        /// </summary>
        public TenantInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TenantInfo class.
        /// </summary>
        /// <param name="id">Unique identifier representing a tenant</param>
        public TenantInfo(string id = null)
        {
            this.Id = id;
        }
    }
}
