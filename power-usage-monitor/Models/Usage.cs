using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class Usage
    {
        public DateTime Time { get; set; }
        public int DeviceId { get; set; }
        public double PowerUsed { get; set; }

        public virtual Device Device { get; set; } = null!;
    }
}
