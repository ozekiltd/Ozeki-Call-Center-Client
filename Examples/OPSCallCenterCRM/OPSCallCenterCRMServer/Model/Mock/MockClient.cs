
using System;
using System.Collections.Generic;
using OPSSDK;
using OPSSDKCommon.Model;
using OPSSDKCommon.Model.Extension;
using OzCommon.Model;
using Ozeki.VoIP;

namespace OPSCallCenterCRMServer.Model.Mock
{
    class MockClient : IClient
    {
        public event EventHandler<ErrorInfo> ErrorOccurred;
        public event EventHandler PhoneBookItemsChanged;
        public event EventHandler<VoIPEventArgs<ISession>> SessionCreated;
        public event EventHandler<VoIPEventArgs<ISession>> SessionCompleted;
        public bool Login(string serverAddress, string username, string password)
        {
            return true;
        }

        public void LoginAsync(string serverAddress, string username, string password, Action<bool> completed)
        {
            completed(true);
        }

        public List<PhoneBookItem> GetPhoneBook()
        {
            throw new NotImplementedException();
        }

        public void GetPhoneBookAsync(Action<List<PhoneBookItem>> completed)
        {
            throw new NotImplementedException();
        }

        public List<ExtensionInfo> GetExtensionInfos()
        {
            throw new NotImplementedException();
        }

        public void GetExtensionInfosAsync(Action<List<ExtensionInfo>> completed)
        {
            throw new NotImplementedException();
        }

        public IAPIExtension GetAPIExtension(string extensionName)
        {
            throw new NotImplementedException();
        }

        public void GetAPIExtensionAsync(string extensionName, Action<IAPIExtension> completed)
        {
            throw new NotImplementedException();
        }
    }
}
