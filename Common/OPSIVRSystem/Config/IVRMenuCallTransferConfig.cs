using System;
using System.Collections.Generic;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.Config
{
    [Serializable]
    public class IVRMenuCallTransferConfig : IVRMenuBaseConfig
    {
        public List<TransferDestination> TransferDestinations { get; set; }

        public IVRMenuCallTransferConfig()
        {
            TransferDestinations=new List<TransferDestination>();
        }
    }
}
