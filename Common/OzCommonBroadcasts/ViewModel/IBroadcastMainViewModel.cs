using System;
using System.ComponentModel;
using OzCommon.Model;

namespace OzCommonBroadcasts.ViewModel
{
    public interface IBroadcastMainViewModel
    {
        ApplicationInformation GetApplicationInformation();
        void CancelLoading();
        Type GetEntryType();
        event PropertyChangedEventHandler PropertyChanged;
        Int32 CheckedJobs { get; }
    }
}
