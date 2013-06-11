using System.Collections.Generic;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.Config;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.Utils
{
    public class ConverterMenu
    {

        public static List<VmIVRMenuElementBase> GetMenuViewModels(List<IVRMenuElementBase> menus)
        {
            List<VmIVRMenuElementBase> reslist = new List<VmIVRMenuElementBase>();
            foreach (var ivrMenuElementBase in menus)
            {
                reslist.Add(GetMenuViewModel(ivrMenuElementBase));
            }
            return reslist;
        }

        public static List<IVRMenuElementBase> ConvertToMenuModels(List<VmIVRMenuElementBase> vmMenuModels)
        {
            List<IVRMenuElementBase> reslist = new List<IVRMenuElementBase>();
            foreach (var vmIvrMenuElementBase in vmMenuModels)
            {
                reslist.Add(vmIvrMenuElementBase.GetModel());
            }
            return reslist;
        }

        public static VmIVRMenuElementBase GetMenuViewModel(IVRMenuElementBase menu)
        {
            VmIVRMenuElementBase result;
            if (menu is IVRMenuElementCallTransfer)
            {
                result = new VmIVRMenuElementCallTransfer((IVRMenuElementCallTransfer)menu);
            }
            else if (menu is IVRMenuElementInfoReader)
            {
                result = new VmIVRMenuElementInfoReader((IVRMenuElementInfoReader)menu);
            }
            else 
            {
                result = new VmIVRMenuElementVoiceMessageRecorder((IVRMenuElementVoiceMessageRecorder)menu);
            }
            return result;
        }

    }
}
