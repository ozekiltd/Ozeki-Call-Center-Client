using System;
using OPSIVRSystem.Utils;
using OPSSDK;
using Ozeki.VoIP;

namespace OPSIVRSystem
{
    public interface IIVREngine
    {
        void Start(string IVRProjectPath, IAPIExtension extension);

        void Stop();

      
        /// <summary>
        /// Logging request on GUI.
        /// </summary>
         event EventHandler<GenericEventArgs<string>> NotifyAction;
    }
}
