using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class Abnormal
    {
        public int AbnormalId { get; set; }
        public int DeviceId { get; set; }
        public double AbnormalUsage { get; set; }
        public DateTime AbnormalTime { get; set; }
        public string? NoticedUser { get; set; }

        public virtual Device Device { get; set; } = null!;
    }
}
