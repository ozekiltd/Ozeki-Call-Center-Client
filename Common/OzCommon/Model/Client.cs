using System;
using System.Collections.Generic;
using OPSSDK;
using OPSSDKCommon.Model;
using OPSSDKCommon.Model.Extension;
using Ozeki.VoIP;

namespace OzCommon.Model
{
    public class Client : IClient
    {
        private OpsClient _client;

        public event EventHandler<ErrorInfo> ErrorOccurred;

        public event EventHandler PhoneBookItemsChanged
        {
            add { _client.PhoneBookItemsChanged += value; }
            remove { _client.PhoneBookItemsChanged -= value; }
        }

        public event EventHandler<VoIPEventArgs<ISession>> SessionCreated
        {
            add { _client.SessionCreated += value; }
            remove { _client.SessionCreated -= value; }
        }

        public event EventHandler<VoIPEventArgs<ISession>> SessionCompleted
        {
            add { _client.SessionCompleted += value; }
            remove { _client.SessionCompleted -= value; }
        }

        public bool Login(string serverAddress, string username, string password)
        {
            if (_client != null)
                _client.ErrorOccurred -= _client_ErrorOccurred;

            _client = new OpsClient();
            _client.ErrorOccurred += _client_ErrorOccurred;

            return _client.Login(serverAddress, username, password);
        }

        public void LoginAsync(string serverAddress, string username, string password, Action<bool> completed)
        {
            if (_client != null)
                _client.ErrorOccurred -= _client_ErrorOccurred;

            _client = new OpsClient();

            _client.ErrorOccurred +=_client_ErrorOccurred;
            _client.LoginAsync(serverAddress, username, password, completed);
        }

        void _client_ErrorOccurred(object sender, ErrorInfo info)
        {
            var handler = ErrorOccurred;
            if (handler != null)
                handler(this, info);
        }

        public List<PhoneBookItem> GetPhoneBook()
        {
            return _client.GetPhoneBook();
        }

        public void GetPhoneBookAsync(Action<List<PhoneBookItem>> completed)
        {
            _client.GetPhoneBookAsync(completed);
        }

        public List<ExtensionInfo> GetExtensionInfos()
        {
            return _client.GetExtensionInfos();
        }

        public void GetExtensionInfosAsync(Action<List<ExtensionInfo>> completed)
        {
            _client.GetExtensionInfosAsync(completed);
        }

        public IAPIExtension GetAPIExtension(string extensionName)
        {
            return _client.GetAPIExtension(extensionName);
        }

        public void GetAPIExtensionAsync(string extensionName, Action<IAPIExtension> completed)
        {
            _client.GetAPIExtensionAsync(extensionName, completed);
        }
    }
}
