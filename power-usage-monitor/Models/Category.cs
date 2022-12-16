using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class Category
    {
        public Category()
        {
            Devices = new HashSet<Device>();
        }

        public int CategoryId { get; set; }
        public string EngneryName { get; set; } = null!;
        public string DeviceCategoryName { get; set; } = null!;
        public double CategoryAvgPower { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
    }
}
