using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Timers;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRM.ViewModel;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;

namespace OPSCallCenterCRM.Model.WCF
{
    class WcfCrmClient : ICrmClient
    {
        private WcfConnectionProperties _connectionProperties;
        private ClientCredential _clientCredential;
        private Timer _pingTimer;
        private ICrmServer _server;

        public event EventHandler<CallSessionInfo> CrmServerCallReceived;
        public event EventHandler<CallHistoryEntry> CrmServerCallHistoryEntryAdded;
        public event EventHandler<CrmEntry> CrmServerEntryAdded;
        public event EventHandler<CrmEntry> CrmServerEntryModified;
        public event EventHandler<CrmEntry> CrmServerEntryDeleted;

        public WcfCrmClient(WcfConnectionProperties connectionProperties, ClientCredential userCredentials)
        {
            _connectionProperties = connectionProperties;
            _clientCredential = userCredentials;
        }

        public void Connect()
        {
            try
            {
                var binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.None;
                var channelFactory = new DuplexChannelFactory<ICrmServer>(this, binding, _connectionProperties.ConnectionString + "/CrmServer");

                _server = channelFactory.CreateChannel();
                _server.Login(_clientCredential);

                _pingTimer = new Timer(5000);
                _pingTimer.Elapsed += (sender, args) => PingServer();
                _pingTimer.Start();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }
        }

        public void CallReceived(CallSessionInfo session)
        {
            OnCrmServerCallReceived(session);
        }

        public void CallHistoryEntryAdded(CallHistoryEntry entry)
        {
            OnCrmServerCallHistoryEntryAdded(entry);
        }

        public void CrmEntryAdded(CrmEntry entry)
        {
            OnCrmServerEntryAdded(entry);
        }

        public void CrmEntryDeleted(CrmEntry entry)
        {
            OnCrmServerEntryDeleted(entry);
        }

        public void CrmEntryModified(CrmEntry entry)
        {
            OnCrmServerEntryModified(entry);
        }

        public void Ping()
        {
            //nothing to do here
        }

        public void Login(ClientCredential user)
        {
            try
            {
                _server.Login(user);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }
        }

        public void Disconnect(ClientCredential user)
        {
            try
            {
                _server.Disconnect(user);
            }
            catch (Exception) { }
        }

        public void AddEntry(CrmEntry entry)
        {
            try
            {
                _server.AddEntry(entry);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }
        }

        public void DeleteEntry(CrmEntry entry)
        {
            try
            {
                _server.DeleteEntry(entry);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }
        }

        public void ModifyEntry(CrmEntry entry)
        {
            try
            {
                _server.ModifyEntry(entry);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }
        }

        public List<CrmEntry> GetAllCrmEntries()
        {
            try
            {
                return _server.GetAllCrmEntries();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }

            return new List<CrmEntry>();
        }

        private void PingServer()
        {
            try
            {
                _server.Ping();
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, MainViewModel.ShowWcfWarningWindow));
            }
        }

        private void OnCrmServerEntryModified(CrmEntry e)
        {
            EventHandler<CrmEntry> handler = CrmServerEntryModified;
            if (handler != null) handler(this, e);
        }

        private void OnCrmServerEntryDeleted(CrmEntry e)
        {
            EventHandler<CrmEntry> handler = CrmServerEntryDeleted;
            if (handler != null) handler(this, e);
        }

        private void OnCrmServerEntryAdded(CrmEntry e)
        {
            EventHandler<CrmEntry> handler = CrmServerEntryAdded;
            if (handler != null) handler(this, e);
        }

        private void OnCrmServerCallHistoryEntryAdded(CallHistoryEntry e)
        {
            EventHandler<CallHistoryEntry> handler = CrmServerCallHistoryEntryAdded;
            if (handler != null) handler(this, e);
        }

        private void OnCrmServerCallReceived(CallSessionInfo e)
        {
            EventHandler<CallSessionInfo> handler = CrmServerCallReceived;
            if (handler != null) handler(this, e);
        }
    }
}
