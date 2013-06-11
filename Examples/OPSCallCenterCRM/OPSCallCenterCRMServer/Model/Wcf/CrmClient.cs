using System;
using System.Threading;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;

namespace OPSCallCenterCRMServer.Model.Wcf
{
    class CrmClient : ICrmClient
    {
        public event EventHandler Disconnected;

        public ICrmClient Client { get; private set; }
        public ClientCredential Credential { get; private set; }

        public CrmClient(ICrmClient client, ClientCredential credential)
        {
            Credential = credential;
            Client = client;
        }

        public void CallReceived(CallSessionInfo session)
        {
            try
            {
                Client.CallReceived(session);
            }
            catch (Exception)
            {
                OnDisconnected(EventArgs.Empty);
            }
        }

        public void CallHistoryEntryAdded(CallHistoryEntry entry)
        {
            try
            {
                Client.CallHistoryEntryAdded(entry);
            }
            catch (Exception)
            {
                OnDisconnected(EventArgs.Empty);
            }
        }

        public void CrmEntryAdded(CrmEntry entry)
        {
            try
            {
                Client.CrmEntryAdded(entry);
            }
            catch (Exception)
            {
                OnDisconnected(EventArgs.Empty);
            }
        }

        public void CrmEntryDeleted(CrmEntry entry)
        {
            try
            {
                Client.CrmEntryDeleted(entry);
            }
            catch (Exception)
            {
                OnDisconnected(EventArgs.Empty);
            }
        }

        public void CrmEntryModified(CrmEntry entry)
        {
            try
            {
                Client.CrmEntryModified(entry);
            }
            catch (Exception)
            {
                OnDisconnected(EventArgs.Empty);
            }
        }

        public void Ping()
        {
            try
            {
                Client.Ping();
            }
            catch (Exception)
            {
                OnDisconnected(EventArgs.Empty);
            }
        }

        private int disconnected;
        private void OnDisconnected(EventArgs e)
        {
            if(Interlocked.Exchange(ref disconnected, 1) != 0)
                return;

            EventHandler handler = Disconnected;
            if (handler != null) handler(this, e);
        }
    }
}
