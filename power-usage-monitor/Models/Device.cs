using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class Device
    {
        public Device()
        {
            Abnormals = new HashSet<Abnormal>();
            Statistics = new HashSet<Statistic>();
            Usages = new HashSet<Usage>();
        }

        public int DeviceId { get; set; }
        public string DeviceName { get; set; } = null!;
        public string StandbyTime { get; set; } = null!;
        public string UseTime { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? UserName { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual User? UserNameNavigation { get; set; }
        public virtual ICollection<Abnormal> Abnormals { get; set; }
        public virtual ICollection<Statistic> Statistics { get; set; }
        public virtual ICollection<Usage> Usages { get; set; }
    }
}
