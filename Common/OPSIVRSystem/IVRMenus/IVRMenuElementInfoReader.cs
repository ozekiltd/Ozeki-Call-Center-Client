using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPSIVRSystem.Config;

namespace OPSIVRSystem.IVRMenus
{
    [Serializable]
    public class IVRMenuElementInfoReader :IVRMenuElementBase
    {

        #region Constructors
        public IVRMenuElementInfoReader()
        {
            Introduction = "This is the info reader menu";
            Name = "Info reader menu";
        }

        private IVRMenuElementInfoReader(IVRMenuElementInfoReader original)
            : base(original)
        {

        }

        public IVRMenuElementInfoReader(IVRMenuInfoReaderConfig config)
            : base(config)
        {
            InitNarrator();
            Narrator.Stopped += Narrator_IntroductionStoped;
            Narrator.Finished += Narrator_IntroductionFinished;
            Narrator.Starting += Narrator_IntroductionStarting;
        }
        #endregion

        #region Narrator events

        void Narrator_IntroductionStarting(object sender, EventArgs e)
        {
            OnIntroductionStarting(Narrator.GetMediaHandler());
        }

        void Narrator_IntroductionFinished(object sender, EventArgs e)
        {
            OnIntroductionFinished();
        }

        void Narrator_IntroductionStoped(object sender, EventArgs e)
        {
            OnIntroductionStoped(Narrator.GetMediaHandler());
        }

        #endregion

     

        public override void StartIntroduction()
        {
            Narrator.StartNarration(NarratorType == NarratorType.TextToSpeech ? Introduction : AudioFile);
        }

        public override void StopIntroduction()
        {
         //   Narrator.StopNarration();
        }

        public override void RestartIntroduction()
        {
            Narrator.RestartNarration(NarratorType == NarratorType.TextToSpeech ? Introduction : AudioFile);
        }

        public override void CommandReceived(int signal)
        {
            Narrator.StopNarration();
            switch (signal)
            {
                case EXIT:
                    OnIntroductionStoped(Narrator.GetMediaHandler());
                    OnStepIntoMenu(null);
                    return;
                case BACK_TO_PARENT_MENU:
                    OnIntroductionStoped(Narrator.GetMediaHandler());
                    OnStepIntoMenu(Parent);
                    return;
            }

            foreach (var child in ChildMenus)
            {
                if (child.TouchToneKey == signal.ToString())
                {
                    OnIntroductionStoped(Narrator.GetMediaHandler());
                    OnStepIntoMenu(child);
                    return;
                }
            }

            //Wrong signal received so it restarts the playing.
            Narrator.Finished += Narrator_IntroductionFinished;
            RestartIntroduction();
        }

        public override IVRMenuElementBase GetAClone()
        {
            return new IVRMenuElementInfoReader(this);
        }

        public override IVRMenuBaseConfig GetConfig()
        {
            var conf = new IVRMenuInfoReaderConfig();
            SetConfigCommonField(conf);
            return conf;
        }
    }
}
