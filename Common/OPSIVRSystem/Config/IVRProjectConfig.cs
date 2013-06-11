using System;
using System.Collections.Generic;

namespace OPSIVRSystem.Config
{
    [Serializable]
   public class IVRProjectConfig
    {
        public string Name { get; set; }

        public List<IVRMenuBaseConfig> MenuList { get; set; }

       public IVRProjectConfig()
        {
            Name = "Untiltled IVR project";
            MenuList = new List<IVRMenuBaseConfig>();

        }
    }
}
