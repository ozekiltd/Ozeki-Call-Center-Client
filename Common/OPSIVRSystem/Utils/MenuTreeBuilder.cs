using System.Collections.Generic;
using System.Diagnostics;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.Utils
{
    public class MenuTreeBuilder
    {
        public static IVRMenuElementBase BuildTreeAndGetRoots(List<IVRMenuElementBase> actualObjects)
        {
            Dictionary<string, IVRMenuElementBase> lookup = new Dictionary<string, IVRMenuElementBase>();
            actualObjects.ForEach(x => lookup.Add(x.Id, x));
            foreach (var item in lookup.Values)
            {
                IVRMenuElementBase proposedParent;
                if (lookup.TryGetValue(item.ParentId, out proposedParent))
                {
                    item.Parent = proposedParent;
                    proposedParent.ChildMenus.Add(item);
                }
            }

            foreach (var item in lookup.Values)
            {
                if (item.ParentId == "")
                {
                    Debug.WriteLine(item.Introduction);
                    return item;
                }
            }
            return null;
        }

        public static VmIVRMenuElementBase BuildTreeAndGetRoots(List<VmIVRMenuElementBase> actualObjects)
        {
            Dictionary<string, VmIVRMenuElementBase> lookup = new Dictionary<string, VmIVRMenuElementBase>();
            actualObjects.ForEach(x => lookup.Add(x.Id, x));
            foreach (var item in lookup.Values)
            {
                VmIVRMenuElementBase proposedParent;
                if (lookup.TryGetValue(item.ParentId, out proposedParent))
                {
                    item.Parent = proposedParent;
                    proposedParent.ChildMenus.Add(item);
                }
            }

            foreach (var item in lookup.Values)
            {
                if (item.ParentId == IVRMenuElementBase.RootIdentifier)
                {
                    Debug.WriteLine(item.Introduction);
                    return item;
                }
            }
            return null;
        }

    }
}
