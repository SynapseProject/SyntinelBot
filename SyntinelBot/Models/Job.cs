using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyntinelBot.Models
{
    public class Job
    {
        public string Id { get; set; }

        public string User { get; set; } // User Alias

        public string Action { get; set; }

        public string Target { get; set; }

        public string From { get; set; } // For EC2 Resizing

        public string To { get; set; } // For EC2 Resizing

        public DateTime JobTime { get; set; }

        public JobStatus Status { get; set; }
    }

    public enum JobStatus
    {
        InProgress,
        Completed,
        Aborted,
        Errored
    }
}
