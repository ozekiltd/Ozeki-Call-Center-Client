using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ozeki.Media.MediaHandlers;

namespace OPSIVRSystem.IVRMenus
{
    class IVRMenuNarratorTextToSpeech :IVRMenuNarrator
    {
        /// <summary>
        /// text to speech reader.
        /// </summary>
        private TextToSpeech txtToSpeech;


        public override void StartNarration(string textInfo)
        {
            txtToSpeech = new TextToSpeech(new Ozeki.VoIP.Media.AudioFormat(8000, 1, 16, 20));
            txtToSpeech.Stopped += new EventHandler<EventArgs>(txtToSpeech_Stopped);
            OnNarrationStarting();
            txtToSpeech.AddText(textInfo);
            txtToSpeech.StartStreaming();
        }
        public override void StopNarration()
        {
            txtToSpeech.StopStreaming();
        }

        public override void RestartNarration(string textInfo)
        {
            txtToSpeech.AddText(textInfo);
            txtToSpeech.StartStreaming();
        }

        public override VoIPMediaHandler GetMediaHandler()
        {
            return txtToSpeech;
        }

        void txtToSpeech_Stopped(object sender, EventArgs e)
        {
            OnNarrationFinished();
        }
    }
}
