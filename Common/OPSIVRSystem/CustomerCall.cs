using System;
using System.Timers;
using OPSIVRSystem.IVRMenus;
using OPSSDK;
using OPSSDKCommon.Model.Call;
using Ozeki.VoIP;


namespace OPSIVRSystem
{
    class CustomerCall
    {
        public event EventHandler<VoIPEventArgs<string>> NotifyAction;
        public event EventHandler Closing;


        public ICall PhoneCall { get; private set; }

        private IVRMenuElementBase currentMenu;

        private MediaManager mediaManager;

        protected Timer repeatTimer;

        public CustomerCall(ICall call, IVRMenuElementBase rootmenu)
        {
            mediaManager = new MediaManager(call);

            repeatTimer = new Timer(5000);
            repeatTimer.AutoReset = false;
            repeatTimer.Elapsed += new ElapsedEventHandler(repeatTimer_Elapsed);
            currentMenu = rootmenu;
            SubscribeCurrentMenuEvents();

            PhoneCall = call;
            PhoneCall.CallErrorOccurred += (PhoneCall_CallErrorOccured);
            PhoneCall.DtmfReceived += PhoneCall_DtmfReceived;
            PhoneCall.CallStateChanged += (PhoneCall_CallStateChanged);
        }



        void PhoneCall_CallStateChanged(object sender, VoIPEventArgs<CallState> e)
        {
            switch (e.Item)
            {
                case CallState.InCall:
                    OnNotifyAction(string.Format("Caller '{0}' is in '{1}' menu.", PhoneCall.OtherParty, currentMenu.Name));
                    currentMenu.StartIntroduction();

                    break;
                case CallState.Completed:
                    OnNotifyAction(string.Format("{0} hanged up the call.", PhoneCall.OtherParty));
                    currentMenu.StopIntroduction();
                    Close();
                    break;
                case CallState.Error:
                case CallState.Rejected:
                case CallState.Cancelled:
                case CallState.Busy:
                    Close();
                    break;
                case CallState.Transferring:
                    OnNotifyAction(string.Format("{0} call is transfering.",PhoneCall.OtherParty));
                    break;
            }
        }


        void PhoneCall_CallErrorOccured(object sender, VoIPEventArgs<CallError> e)
        {
            OnNotifyAction(string.Format("Some error occure in the following {0} call", PhoneCall));
        }


        void PhoneCall_DtmfReceived(object sender, VoIPEventArgs<DtmfSignal> e)
        {
            DtmfSignal dtmfSignal = e.Item;

            OnNotifyAction(string.Format("{0} sent the following DTMF sign: {1}", PhoneCall.OtherParty, (int)dtmfSignal.Signal));
            currentMenu.CommandReceived((int)dtmfSignal.Signal);
        }

        private void SubscribeCurrentMenuEvents()
        {
            currentMenu.IntroductionStarting += (currentMenu_IntroductionStarting);
            currentMenu.IntroductionStoped += (currentMenu_IntroductionStoped);
            currentMenu.IntroductionFinished += (currentMenu_IntroductionFinished);
            currentMenu.StepIntoMenu += (currentMenu_StepIntoMenu);
            currentMenu.CallTransferRequired += currentMenu_CallTransferRequired;
        }

        private void UnSubscribeCurrentMenuEvents()
        {
            currentMenu.IntroductionStarting -= (currentMenu_IntroductionStarting);
            currentMenu.IntroductionStoped -= (currentMenu_IntroductionStoped);
            currentMenu.IntroductionFinished -= (currentMenu_IntroductionFinished);
            currentMenu.StepIntoMenu -= (currentMenu_StepIntoMenu);
            currentMenu.CallTransferRequired -= currentMenu_CallTransferRequired;
        }

        void currentMenu_StepIntoMenu(object sender, VoIPEventArgs<IVRMenuElementBase> e)
        {
            repeatTimer.Stop();
            if (e.Item == null)//If event param is null it's indicates the exit command, close the call.
            {
                PhoneCall.HangUp();
                Close();
                return;
            }

            UnSubscribeCurrentMenuEvents();
            currentMenu = e.Item;
            OnNotifyAction(string.Format("Caller '{0}' is in '{1}' menu.", PhoneCall.OtherParty, currentMenu.Name));
            SubscribeCurrentMenuEvents();
            currentMenu.StartIntroduction();
        }

        void currentMenu_IntroductionFinished(object sender, EventArgs e)
        {
            repeatTimer.Start();
        }

        void repeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentMenu.RestartIntroduction();
        }

        void currentMenu_IntroductionStoped(object sender, VoIPEventArgs<Ozeki.Media.MediaHandlers.VoIPMediaHandler> e)
        {
            mediaManager.DeAttachVoIPHandlerToCall(e.Item);
        }

        void currentMenu_IntroductionStarting(object sender, VoIPEventArgs<Ozeki.Media.MediaHandlers.VoIPMediaHandler> e)
        {
            mediaManager.AttachVoIPHandlerToCall(e.Item);
        }

        void currentMenu_CallTransferRequired(object sender, VoIPEventArgs<string> e)
        {
            OnNotifyAction(string.Format("It is transefering the call '{0}' to '{1}'", PhoneCall.OtherParty, e.Item));
            PhoneCall.BlindTransfer(e.Item);
        }

        private void OnNotifyAction(string action)
        {
            if (NotifyAction != null)
                NotifyAction(this, new VoIPEventArgs<string>(action));
        }

        private void Close()
        {
            if (Closing != null)
            {
                Closing(this, new EventArgs());
            }
            PhoneCall.CallStateChanged -= PhoneCall_CallStateChanged;
            PhoneCall.CallErrorOccurred -= PhoneCall_CallErrorOccured;
            PhoneCall.DtmfReceived -= PhoneCall_DtmfReceived;
            UnSubscribeCurrentMenuEvents();
        }

        public override string ToString()
        {
            if (PhoneCall == null)
                return base.ToString();
            return PhoneCall.ToString();
        }
    }
}
