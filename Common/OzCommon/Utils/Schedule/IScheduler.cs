using System;
using System.Collections.Generic;

namespace OzCommon.Utils.Schedule
{
    public interface IScheduler<T> where T : class
    {
        event EventHandler<CounterEventArg> WorksCompleted;
        event EventHandler OneWorkCompleted;

        bool Working { get; }
        int MaxConcurrentWorkers { get; set; }
        void SetMaxConcurrentWorkers(Int32 count);

        void StartWorks(IList<T> jobsTodo);
        void StopWorks();
    }
}
