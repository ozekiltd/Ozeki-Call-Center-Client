using System.Collections.Generic;
using OPSIVRSystem.Config;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.CommonViewModel
{
    public class VmIVRMenuElementCallTransfer :VmIVRMenuElementBase
    {

        public List<TransferDestination> TransferDestinations { get; set; }

        public VmIVRMenuElementCallTransfer()
        {
            Icon = "/OPSIVRSystem;component/Resources/transfer.png";
            Introduction = "This is a call transfer menu";
            Name = "Call transfer menu element";
            TypeText = "Call transfer menu";
            InitDestinations();
        }

        private void InitDestinations()
        {
            TransferDestinations = new List<TransferDestination>();
            for (int i = 1; i < 10; i++)
            {
                TransferDestinations.Add(new TransferDestination() { Destination = string.Empty });
            }

        }
           private VmIVRMenuElementCallTransfer(VmIVRMenuElementCallTransfer original)
            : base(original)
        {
               TransferDestinations=new List<TransferDestination>(original.TransferDestinations);
        }

           public VmIVRMenuElementCallTransfer(IVRMenuElementCallTransfer model)
               : base(model)
           {
               Icon = "/OPSIVRSystem;component/Resources/transfer.png"; 
               TransferDestinations=new List<TransferDestination>(model.TransferDestinations);
           }

           public VmIVRMenuElementCallTransfer(IVRMenuCallTransferConfig model)
               : base(model)
           {
               Icon = "/OPSIVRSystem;component/Resources/transfer.png"; 
               TransferDestinations=new List<TransferDestination>(model.TransferDestinations);
           }

        

        public override VmIVRMenuElementBase GetAClone()
        {
            return new VmIVRMenuElementCallTransfer(this);
        }

        public override IVRMenuElementBase GetModel()
        {
            var res = new IVRMenuElementCallTransfer();
            InitModelCommonFields(res);
            res.TransferDestinations = TransferDestinations;
            return res;
        }
    }
}
