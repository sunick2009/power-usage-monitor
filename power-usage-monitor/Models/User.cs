using System;
using System.Collections.Generic;

namespace power_usage_monitor.Models
{
    public partial class User
    {
        public User()
        {
            Devices = new HashSet<Device>();
        }

        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;

        public virtual ICollection<Device> Devices { get; set; }
    }
}
