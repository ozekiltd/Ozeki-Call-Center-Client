using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;

namespace OPSIVRSystem.IVRMenus
{
    public abstract class IVRMenuNarrator
    {
        public abstract void StartNarration(string textInfo);

        public abstract void StopNarration();

        public abstract void RestartNarration(string textInfo);

        public abstract VoIPMediaHandler GetMediaHandler();
        
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Finished;


        

        /// <summary>
        /// Indicates the narrator starts the reading.
        /// </summary>
        /// <param name="voiphandler"></param>
        protected void OnNarrationStarting()
        {
            if (Starting != null)
                Starting(this, new EventArgs());
        }

      
        /// <summary>
        /// Indicates the narrator stops the reading.
        /// </summary>
        /// <param name="voiphandler"></param>
        protected void OnNarrationStopped()
        {
            if (Stopped != null)
                Stopped(this, new EventArgs());
        }

        /// <summary>
        /// Indicates the current menu introduction reading has finished.
        /// </summary>
        protected void OnNarrationFinished()
        {
            if (Finished != null)
                Finished(this, new EventArgs());
        }


        public static IVRMenuNarrator CreateNarrator(NarratorType type)
        {
            switch (type)
            {
                    case NarratorType.TextToSpeech:
                    return new IVRMenuNarratorTextToSpeech();
                    case  NarratorType.FilePlayback:
                    return new IVRMenuNarratorFilePlayback();
                default:
                    throw new Exception("The given Narrator type is not a supported narrator type.");
            }
        }
    }
}
