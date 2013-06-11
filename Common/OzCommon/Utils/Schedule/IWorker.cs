using System;

namespace OzCommon.Utils.Schedule
{
    public interface IWorker
    {
        event EventHandler<WorkResult> WorkCompleted;
        void StartWork();
        void CancelWork();
    }
}
