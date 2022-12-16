using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class Statistic
    {
        public int DeviceId { get; set; }
        public string Period { get; set; } = null!;
        public double TotalUsage { get; set; }
        public double AveragePower { get; set; }

        public virtual Device Device { get; set; } = null!;
    }
}
