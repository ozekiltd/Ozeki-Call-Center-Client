using System;
using System.Collections.Generic;
using OzCommon.Utils;

namespace OzCommonBroadcasts.Model.Csv  
{
    public interface ICsvExporter<T> where T : class
    {
        void ExportToFile(List<T> objects, string path);
        bool IsSaving { get; }
        void Cancel();
        event EventHandler<CounterEventArg> AllItemSaved;
    }
}
