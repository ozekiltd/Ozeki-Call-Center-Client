using OPSIVRSystem.Config;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.CommonViewModel
{
    public class VmIVRMenuElementInfoReader : VmIVRMenuElementBase
    {
        
        public VmIVRMenuElementInfoReader()
        {
            Icon = "/OPSIVRSystem;component/Resources/texttospeech.png";
            Introduction = "This is the info reader menu";
            Name = "Info reader menu element";
            TypeText = "Info reader menu";
           
        }

        private VmIVRMenuElementInfoReader(VmIVRMenuElementInfoReader original)
            : base(original)
        {

        }

        public VmIVRMenuElementInfoReader(IVRMenuElementInfoReader model)
            : base(model)
        {
            Icon = "/OPSIVRSystem;component/Resources/texttospeech.png";
        }


        public VmIVRMenuElementInfoReader(IVRMenuInfoReaderConfig model)
            : base(model)
        {
            Icon = "/OPSIVRSystem;component/Resources/texttospeech.png";
        }

        public override VmIVRMenuElementBase GetAClone()
        {
            return new VmIVRMenuElementInfoReader(this);
        }

        public override IVRMenuElementBase GetModel()
        {
            var res = new IVRMenuElementInfoReader();
            InitModelCommonFields(res);
            return res;
        }
    }
}
