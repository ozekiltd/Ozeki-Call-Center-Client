using System;
using System.Collections.Generic;
using OPSIVRSystem.Config;

namespace OPSIVRSystem.IVRMenus
{
    [Serializable]
    public class IVRMenuElementCallTransfer : IVRMenuElementBase
    {
        public List<TransferDestination> TransferDestinations { get; set; }

        #region Constructors
        public IVRMenuElementCallTransfer()
        {
            Introduction = "This is a call transfer menu";
            Name = "Call transfer menu";
          InitDestinations();
        }

        private IVRMenuElementCallTransfer(IVRMenuElementCallTransfer original)
            : base(original)
        {
            TransferDestinations = new List<TransferDestination>(original.TransferDestinations);
        }

        public IVRMenuElementCallTransfer(IVRMenuCallTransferConfig config)
            : base(config)
        {
            InitNarrator();
            Narrator.Stopped += Narrator_IntroductionStoped;
            Narrator.Finished += Narrator_IntroductionFinished;
            Narrator.Starting += Narrator_IntroductionStarting;
            TransferDestinations = new List<TransferDestination>(config.TransferDestinations);
        }

        private void InitDestinations()
        {
            TransferDestinations = new List<TransferDestination>();
            for (int i = 1; i < 10; i++)
            {
                TransferDestinations.Add(new TransferDestination() { Destination = string.Empty });
            }
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
            Narrator.StopNarration();
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
            for (int i = 1; i < 10; i++)
            {
                if (signal == i && !string.IsNullOrEmpty(TransferDestinations[i-1].Destination))
                {
                    OnCallTransferRequired(TransferDestinations[i-1].Destination);
                    return;
                }
            }
            RestartIntroduction();
        }

        public override IVRMenuElementBase GetAClone()
        {
            return new IVRMenuElementCallTransfer(this);
        }

        public override IVRMenuBaseConfig GetConfig()
        {
            var conf = new IVRMenuCallTransferConfig();
            SetConfigCommonField(conf);
            conf.TransferDestinations = new List<TransferDestination>(TransferDestinations);
            return conf;
        }
    }
}
