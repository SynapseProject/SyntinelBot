using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace SyntinelBot
{
    public class ServiceState : BotState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceState"/> class to be shared across different turn context.
        /// </summary>
        /// <param name="storage">The storage provider to use.</param>
        public ServiceState(IStorage storage)
            : base(storage, nameof(ServiceState))
        {
        }

        protected override string GetStorageKey(ITurnContext turnContext)
        {
            return "service/shared";
        }
    }
}
