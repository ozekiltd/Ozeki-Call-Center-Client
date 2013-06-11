using System;
using System.Collections.Generic;
using OPSSDK;
using OPSSDKCommon.Model;
using OPSSDKCommon.Model.Extension;
using Ozeki.VoIP;

namespace OzCommon.Model
{
    public interface IClient
    {
        event EventHandler<ErrorInfo> ErrorOccurred;
        event EventHandler PhoneBookItemsChanged;
        event EventHandler<VoIPEventArgs<ISession>> SessionCreated;
        event EventHandler<VoIPEventArgs<ISession>> SessionCompleted;

        bool Login(string serverAddress, string username, string password);
        void LoginAsync(string serverAddress, string username, string password, Action<bool> completed);
        List<PhoneBookItem> GetPhoneBook();
        void GetPhoneBookAsync(Action<List<PhoneBookItem>> completed);
        List<ExtensionInfo> GetExtensionInfos();
        void GetExtensionInfosAsync(Action<List<ExtensionInfo>> completed);
        IAPIExtension GetAPIExtension(string extensionName);
        void GetAPIExtensionAsync(string extensionName, Action<IAPIExtension> completed);
    }
}
