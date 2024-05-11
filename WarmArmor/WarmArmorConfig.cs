using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarmArmor
{
    public class WarmArmorConfig
    {
        public static WarmArmorConfig Loaded { get; set; } = new WarmArmorConfig();

        public bool PatchEntityBehaviorBodyTemperature { get; set; } = true;

        public bool PatchItemWearable { get; set; } = true;
    }
}
