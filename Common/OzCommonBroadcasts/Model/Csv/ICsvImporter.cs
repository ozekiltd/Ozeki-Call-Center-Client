using System;
using OzCommon.Utils;

namespace OzCommonBroadcasts.Model.Csv
{
    public interface ICsvImporter<T> where T : EventArgs
    {
        void LoadFile(string path);
        void Cancel();
        bool IsLoading { get; }
        event EventHandler<T> ItemLoaded;
        event EventHandler<CounterEventArg> AllItemLoaded;
    }
}
