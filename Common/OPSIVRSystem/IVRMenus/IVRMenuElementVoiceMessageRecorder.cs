using System;
using System.Diagnostics;
using System.IO;
using OPSIVRSystem.Config;
using Ozeki.Media.MediaHandlers;

namespace OPSIVRSystem.IVRMenus
{
    /// <summary>
    /// This IVR menu class records a voice message in uncrompressed wav format.
    /// </summary>
    [Serializable]
    public class IVRMenuElementVoiceMessageRecorder : IVRMenuElementBase
    {

        /// <summary>
        /// The recorder object
        /// </summary>
        private WaveStreamRecorder waveStreamRecorder;

        public string PostIntroduction { get; set; }

        public string PostIntroductionAudio { get; set; }   


        #region Constructors
        public IVRMenuElementVoiceMessageRecorder()
        {
            Introduction = "This is a voice message recorder menu";
            Name = "Voice message recorder menu";
            PostIntroduction = "Thank you bye";
            PostIntroduction = "";
        }

        private IVRMenuElementVoiceMessageRecorder(IVRMenuElementVoiceMessageRecorder original)
            : base(original)
        {
            PostIntroduction = original.PostIntroduction;
            PostIntroductionAudio = original.PostIntroduction;
        }

        public IVRMenuElementVoiceMessageRecorder(IVRMenuVoiceMessageRecorderConfig config)
            : base(config)
        {
            PostIntroduction = config.PostIntroduction;
            PostIntroductionAudio = config.PostIntroductionAudio;
               InitNarrator();
              Narrator.Finished += Narrator_IntroductionFinished;
              Narrator.Stopped += Narrator_Stopped;
              Narrator.Starting += Narrator_Starting;
        }

      

        #endregion


        #region Narrator events

        void Narrator_Starting(object sender, EventArgs e)
        {
            OnIntroductionStarting(Narrator.GetMediaHandler());
        }

        void Narrator_Stopped(object sender, EventArgs e)
        {
            Debug.WriteLine("Recording megállítottaák");
        }

        /// <summary>
        /// Indicates the introduction has finished. It's starts the voice record.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Narrator_IntroductionFinished(object sender, EventArgs e)
        {
            Narrator.Finished -= Narrator_IntroductionFinished;

            waveStreamRecorder =
                new WaveStreamRecorder(Path.Combine(Environment.CurrentDirectory,
                                                    string.Format("IVRVoiceMesage{0}.wav", DateTime.Now.ToFileTime())));

            OnIntroductionStarting(waveStreamRecorder);
            waveStreamRecorder.StartStreaming();

            Narrator.Finished += narrator_PostIntroductionFinished;
        }

        /// <summary>
        /// Indicates the post message has read. The call ends.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void narrator_PostIntroductionFinished(object sender, EventArgs e)
        {
            Narrator.Finished -= narrator_PostIntroductionFinished;
            OnIntroductionStoped(Narrator.GetMediaHandler());
            Narrator.Finished += Narrator_IntroductionFinished;
            OnStepIntoMenu(null);
        }

        #endregion
       

        public override void StartIntroduction()
        {
            Narrator.StartNarration(NarratorType == NarratorType.TextToSpeech ? Introduction : AudioFile);
        }

        public override void StopIntroduction()
        {
            CloseRecorder();
            CloseNarrator();
        }

        public override void RestartIntroduction()
        {
            Narrator.RestartNarration(NarratorType == NarratorType.TextToSpeech ? Introduction : AudioFile);
        }

        public override void CommandReceived(int signal)
        {
            switch (signal)
            {
                case EXIT:
                    CloseRecorder();
                    OnStepIntoMenu(null);
                    return;
                case BACK_TO_PARENT_MENU:
                    CloseRecorder();
                    CloseNarrator();

                    OnStepIntoMenu(Parent);
                    return;
                case INPUT_END:
                    CloseRecorder();
                    Narrator.RestartNarration(NarratorType == NarratorType.TextToSpeech ? PostIntroduction : PostIntroductionAudio);
                    return;
            }
        }

   
        /// <summary>
        /// Releases the recorder object
        /// </summary>
        private void CloseRecorder()
        {
            try
            {
                if (waveStreamRecorder != null)
                {
                    waveStreamRecorder.StopStreaming();
                    OnIntroductionStoped(waveStreamRecorder);
                    waveStreamRecorder.StopStreaming();
                    waveStreamRecorder.Dispose();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Release the reader object.
        /// </summary>
        private void CloseNarrator()
        {
            Narrator.StopNarration();
            OnIntroductionStoped(Narrator.GetMediaHandler());
        }

        public override IVRMenuElementBase GetAClone()
        {
            return new IVRMenuElementVoiceMessageRecorder(this);
        }

        public override IVRMenuBaseConfig GetConfig()
        {
            var conf = new IVRMenuVoiceMessageRecorderConfig();
            SetConfigCommonField(conf);
            conf.PostIntroduction = PostIntroduction;
            conf.PostIntroductionAudio = PostIntroductionAudio;
            return conf;
        }
    }
}

