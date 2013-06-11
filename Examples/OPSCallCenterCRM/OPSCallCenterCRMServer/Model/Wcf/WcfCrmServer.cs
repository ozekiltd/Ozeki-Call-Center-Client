using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;
using OPSCallCenterCRMServer.Model.Database;
using OPSSDK;
using OzCommon.Model;
using Ozeki.VoIP;

namespace OPSCallCenterCRMServer.Model.Wcf
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class WcfCrmServer : ICrmServer
    {
        public static event EventHandler<NotificationEventArgs> NotificationReceived;

        private static ConcurrentDictionary<ICrmClient, CrmClient> _clients;
        private static IDatabaseClient<CrmEntry> _databaseClient;
        private static IClient _client;

        public WcfCrmServer()
        {
            _clients = new ConcurrentDictionary<ICrmClient, CrmClient>();
            _databaseClient = SimpleIoc.Default.GetInstance<IDatabaseClient<CrmEntry>>();
            _client = SimpleIoc.Default.GetInstance<IClient>();
            _client.SessionCompleted += ClientOnSessionCompleted;
            _client.SessionCreated += ClientOnSessionCreated;
        }

        public void Login(ClientCredential user)
        {
            var callback = GetContext();
            var client = new CrmClient(callback, user);

            if (_clients.TryAdd(callback, client))
                client.Disconnected += ClientDisconnected;

            OnNotificationReceived(new NotificationEventArgs() { Notification = string.Format("New client connected: {0} ({1})", user.UserName, user.PhoneNumber) });
        }

        public void Disconnect(ClientCredential user)
        {
            if (user == null)
                return;

            var client = _clients.FirstOrDefault(cl => cl.Value.Credential.PhoneNumber.Equals(user.PhoneNumber) && cl.Value.Credential.UserName.Equals(user.UserName));
            if (client.Key != null)
            {
                CrmClient removedClient;
                _clients.TryRemove(client.Key, out removedClient);

                OnNotificationReceived(new NotificationEventArgs() { Notification = string.Format("Client disconnected: {0} ({1})", user.UserName, user.PhoneNumber) });
            }
        }

        private void ClientOnSessionCompleted(object sender, VoIPEventArgs<ISession> voIpEventArgs)
        {
            var callHistoryentry = new CallHistoryEntry();

            callHistoryentry.Callee = voIpEventArgs.Item.Callee;
            callHistoryentry.Caller = voIpEventArgs.Item.Caller;
            callHistoryentry.StartDate = voIpEventArgs.Item.StartTime;
            callHistoryentry.CallLength = voIpEventArgs.Item.TalkDuration;

            try
            {
                var affectedEntries = _databaseClient.GetAll().Where(entry => entry.PhoneNumber.Equals(callHistoryentry.Callee) || entry.PhoneNumber.Equals(callHistoryentry.Caller));
                foreach (var affectedEntry in affectedEntries)
                {
                    affectedEntry.CallHistoryEntries.Add(callHistoryentry);
                    _databaseClient.Set(affectedEntry);
                }

                CallHistoryEntryAdded(callHistoryentry);
            }
            catch (Exception) { }
        }

        private void ClientOnSessionCreated(object sender, VoIPEventArgs<ISession> voIpEventArgs)
        {
            var sessionInfo = new CallSessionInfo();

            sessionInfo.Callee = voIpEventArgs.Item.Callee;
            sessionInfo.Caller = voIpEventArgs.Item.Caller;
            sessionInfo.RingDuration = voIpEventArgs.Item.RingDuration;
            sessionInfo.StartTime = voIpEventArgs.Item.StartTime;
            sessionInfo.TalkDuration = voIpEventArgs.Item.TalkDuration;

            CallReceived(sessionInfo);
        }

        public void AddEntry(CrmEntry entry)
        {
            try
            {
                _databaseClient.Set(entry);

                CrmClient currentClient;
                if (!_clients.TryGetValue(GetContext(), out currentClient))
                    return;

                foreach (var crmClient in _clients.Values)
                {
                    crmClient.CrmEntryAdded(entry);
                }

                OnNotificationReceived(new NotificationEventArgs() { Notification = string.Format("New CRM Entry added from Client {0}: {1} ({2}, {3})", currentClient.Credential.UserName, entry.ID, entry.LastName, entry.FirstName) });
            }
            catch (Exception) { }
        }

        public void DeleteEntry(CrmEntry entry)
        {
            try
            {
                _databaseClient.Delete(entry);

                CrmClient currentClient;
                if (!_clients.TryGetValue(GetContext(), out currentClient))
                    return;

                foreach (var crmClient in _clients.Values)
                {
                    crmClient.CrmEntryDeleted(entry);
                }

                OnNotificationReceived(new NotificationEventArgs() { Notification = string.Format("CRM Entry deleted by Client {0}: {1} ({2}, {3})", currentClient.Credential.UserName, entry.ID, entry.LastName, entry.FirstName) });
            }
            catch (Exception) { }
        }

        public void ModifyEntry(CrmEntry entry)
        {
            try
            {
                _databaseClient.Set(entry);

                CrmClient currentClient;
                if (!_clients.TryGetValue(GetContext(), out currentClient))
                    return;

                foreach (var crmClient in _clients.Values)
                    crmClient.CrmEntryModified(entry);

                OnNotificationReceived(new NotificationEventArgs() { Notification = string.Format("CRM Entry Modified by Client {0}: {1} ({2}, {3})", currentClient.Credential.UserName, entry.ID, entry.LastName, entry.FirstName) });
            }
            catch (Exception) { }
        }

        public List<CrmEntry> GetAllCrmEntries()
        {
            var crmEntries = new List<CrmEntry>();
            crmEntries.AddRange(_databaseClient.GetAll());

            return crmEntries;
        }

        public void Ping()
        {
            try
            {
                CrmClient currentClient;
                if (!_clients.TryGetValue(GetContext(), out currentClient))
                    return;

                currentClient.Ping();
            }
            catch (Exception) { }
        }

        private void CallHistoryEntryAdded(CallHistoryEntry entry)
        {
            try
            {
                foreach (var crmClient in _clients.Values)
                    crmClient.CallHistoryEntryAdded(entry);

                OnNotificationReceived(new NotificationEventArgs() { Notification = string.Format("New call history entry added: {0} -> {1}", entry.Caller, entry.Callee) });
            }
            catch (Exception) { }
        }

        private void CallReceived(CallSessionInfo sessionInfo)
        {
            try
            {
                foreach (var crmClient in _clients.Values.Where(client => client.Credential.PhoneNumber.Equals(sessionInfo.Callee) || client.Credential.PhoneNumber.Equals(sessionInfo.Caller)))
                    crmClient.CallReceived(sessionInfo);
            }
            catch (Exception) { }
        }

        private void ClientDisconnected(object sender, EventArgs e)
        {
            Disconnect(((CrmClient)sender).Credential);
        }

        private ICrmClient GetContext()
        {
            return OperationContext.Current.GetCallbackChannel<ICrmClient>();
        }

        private static void OnNotificationReceived(NotificationEventArgs e)
        {
            EventHandler<NotificationEventArgs> handler = NotificationReceived;
            if (handler != null) handler(null, e);
        }
    }
}
