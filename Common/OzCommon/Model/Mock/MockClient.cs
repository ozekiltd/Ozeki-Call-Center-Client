using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OPSSDK;
using OPSSDKCommon.Model;
using OPSSDKCommon.Model.Extension;
using Ozeki.VoIP;

namespace OzCommon.Model.Mock
{
    public class MockClient : IClient
    {
        public event EventHandler <ErrorInfo> ErrorOccurred;
        public event EventHandler PhoneBookItemsChanged;
        public event EventHandler<VoIPEventArgs<ISession>> SessionCreated;
        public event EventHandler<VoIPEventArgs<ISession>> SessionCompleted;
        public bool Login(string serverAddress, string username, string password)
        {
            Thread.Sleep(1000);
            return true;
        }

        public void LoginAsync(string serverAddress, string username, string password, Action<bool> completed)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                completed(true);
            });
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
