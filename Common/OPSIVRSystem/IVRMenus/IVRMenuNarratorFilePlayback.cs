using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ozeki.Media.MediaHandlers;

namespace OPSIVRSystem.IVRMenus
{
    class IVRMenuNarratorFilePlayback : IVRMenuNarrator
    {

        private string currentAudioFile;
        /// <summary>
        /// wav file player object.
        /// </summary>
        private WaveStreamPlayback waveStreamPlayback;


        public override void StartNarration(string audiofile)
        {
            currentAudioFile = audiofile;
            waveStreamPlayback = new WaveStreamPlayback(audiofile);
            waveStreamPlayback.Stopped += waveStreamPlayback_Stopped;
            OnNarrationStarting();
            waveStreamPlayback.StartStreaming();
        }

        public override void StopNarration()
        {
            waveStreamPlayback.StopStreaming();
        }

        public override void RestartNarration(string audioFile)
        {
            if (!string.IsNullOrEmpty(currentAudioFile) && audioFile != currentAudioFile)
            {
                changeAudioFile(audioFile);
            }
            else
                waveStreamPlayback.StartStreaming();
        }

        public override VoIPMediaHandler GetMediaHandler()
        {
            return waveStreamPlayback;
        }

        /// <summary>
        /// Indicates the introduction has finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void waveStreamPlayback_Stopped(object sender, EventArgs e)
        {
            OnNarrationFinished();
        }

        private void changeAudioFile(string newAudioFile)
        {
            OnNarrationStopped();
            waveStreamPlayback.Stopped -= waveStreamPlayback_Stopped;
            waveStreamPlayback.Dispose();
            StartNarration(newAudioFile);

        }
    }
}
