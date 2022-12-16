using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class Abnormal
    {
        public int AbnormalId { get; set; }
        public int DeviceId { get; set; }
        public double AbnormalUsage { get; set; }
        public string AbnormalTime { get; set; } = null!;
        public string? NoticedUser { get; set; }

        public virtual Device AbnormalNavigation { get; set; } = null!;
        public virtual User? NoticedUserNavigation { get; set; }
    }
}
