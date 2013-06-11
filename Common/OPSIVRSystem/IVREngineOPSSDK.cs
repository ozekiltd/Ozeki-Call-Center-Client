using System;
using System.Collections.Generic;
using OPSIVRSystem.Config;
using OPSIVRSystem.Utils;
using OPSSDK;

using Ozeki.Common.Logger;
using Ozeki.VoIP;

namespace OPSIVRSystem
{
    public class IVREngineOPSSDK : IIVREngine
    {

        /// <summary>
        /// Logging request on GUI.
        /// </summary>
        public event EventHandler<GenericEventArgs<string>> NotifyAction;

        /// <summary>
        /// Collection that contains the current active calls.
        /// </summary>
        private List<CustomerCall> activeCalls;

        private IAPIExtension extension;

        private IVRProject ivrProject;

        private ProjectStore projectStore;

        public IVREngineOPSSDK()
        {
            projectStore = new ProjectStore();
       
            activeCalls = new List<CustomerCall>();
            Logger.Open(LogLevel.Notice);
        }

    
        /// <summary>
        /// Incoming call events that occur in IAPIExtension.
        /// </summary>
        /// <param name="sender">The SoftPhone.</param>
        /// <param name="e">The incomming call.</param>
        void extension_IncomingCall(object sender, VoIPEventArgs<ICall> e)
        {
            OnNotifyAction(string.Format("Incoming call received, caller is {0}", e.Item.OtherParty));
            CustomerCall call = new CustomerCall(e.Item, ivrProject.GetNewMenuroot());
            call.NotifyAction += new EventHandler<VoIPEventArgs<string>>(call_NotifyAction);
            call.Closing += new EventHandler(call_Closing);
            call.PhoneCall.Accept();
            activeCalls.Add(call);
        }

        /// <summary>
        /// An active call ends, so it removes from the active call collection.
        /// </summary>
        /// <param name="sender">CustmerCall object</param>
        /// <param name="e"> Not used.</param>
        void call_Closing(object sender, EventArgs e)
        {
            CustomerCall customerCall = sender as CustomerCall;
            customerCall.NotifyAction -= call_NotifyAction;
            customerCall.Closing -= call_Closing;
            activeCalls.Remove(customerCall);
        }

        /// <summary>
        /// Message received for logging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">message</param>
        void call_NotifyAction(object sender, VoIPEventArgs<string> e)
        {
            OnNotifyAction(e.Item);
        }

        /// <summary>
        /// Sends the current event message to the GUI.
        /// </summary>
        /// <param name="actionMessage">Message.</param>
        private void OnNotifyAction(string actionMessage)
        {
            if (NotifyAction != null)
                NotifyAction(this, new GenericEventArgs<string>(actionMessage));
        }

        #region Implementation of IIVREngine

        public void Start(string IVRProjectPath, IAPIExtension extension)
        {
            ivrProject = (projectStore.LoadProject(IVRProjectPath));
            this.extension = extension;
            this.extension.IncomingCall += extension_IncomingCall;
            OnNotifyAction(string.Format("IVR engine ready to receive call on '{0}' Api extension.",extension.ExtensionId));
        }

        public void Stop()
        {
            if (extension != null)
            {
                OnNotifyAction("IVR engine has stopped.");
                extension.IncomingCall -= extension_IncomingCall;
            }
        }

   
        #endregion
    }
   
}
