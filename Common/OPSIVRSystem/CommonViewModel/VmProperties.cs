
namespace OPSIVRSystem.CommonViewModel
{
    public class VmProperties 
    {
        private VmIVRMenuElementBase _currentIVRMenu;


        //designer miatt
        public VmProperties()
        {
       
        }

        public VmProperties(VmIVRMenuElementBase menu)
        {
            CurrentIVRMenu = menu;
        }

        public VmIVRMenuElementBase CurrentIVRMenu
        {
            get { return _currentIVRMenu; }
            set
            {
                _currentIVRMenu = value;
             //   CreateApropiatePartControl(value);
            }
        }

        //private void CreateApropiatePartControl(VmIVRMenuElementBase menubase)
        //{
        //    if (menubase is VmIVRMenuElementVoiceMessageRecorder)
        //    {
        //        UCMenuSpecificPart = new UcVoiceMessageRecorderProperties();
        //    }
        //    else if (menubase is VmIVRMenuElementCallTransfer)
        //    {
        //        UCMenuSpecificPart = new UcCallTransferProperties();
        //    }
        //}

        //public UserControl UCMenuSpecificPart { get; set; }



    }
}
