using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OPSIVRSystem.IVRMenus;
using OPSIVRSystem.Utils;

namespace OPSIVRSystem
{
    [Serializable]
    public class IVRProject
    {
        private IVRMenuElementBase _ivrMenuRoot;
        public string Name { get; set; }


        public List<IVRMenuElementBase> MenuList { get; set; }

        [XmlIgnore]
        public IVRMenuElementBase IVRMenuRoot
        {
            get
            {
                if (_ivrMenuRoot == null)
                {
                    _ivrMenuRoot = MenuTreeBuilder.BuildTreeAndGetRoots(MenuList);
                }
                return _ivrMenuRoot;
            }
            set { _ivrMenuRoot = value; }
        }

        public IVRMenuElementBase GetNewMenuroot()
        {
            return MenuTreeBuilder.BuildTreeAndGetRoots(MenuList);
        }

        public IVRProject()
        {
            Name = "Untiltled IVR project";
            MenuList = new List<IVRMenuElementBase>();

        }

    }
}
