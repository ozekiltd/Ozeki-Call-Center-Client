using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OzCommon.Utils.Schedule
{
    public class Scheduler<T> : IScheduler<T> where T : class
    {
        private readonly IWorkerFactory<T> workerFactory;
        private ManualResetEvent eventLock;
        private readonly object objectLock;

        private List<IWorker> currentWorkers;  
        
        protected int CurrentConcurrentWorkers;
        protected int CompletedWorks;
        protected int CurrentWork;

        public event EventHandler<CounterEventArg> WorksCompleted;
        public event EventHandler OneWorkCompleted;

        public int MaxConcurrentWorkers { get; set; }
        public bool Working { get; protected set; }

        public Scheduler(IWorkerFactory<T> workerFactory)
        {
            this.workerFactory = workerFactory;

            currentWorkers = new List<IWorker>();

            eventLock = new ManualResetEvent(true);
            objectLock = new object();
        }

        public virtual void StartWorks(IList<T> jobsTodo)
        {
            lock (objectLock)
            {
                if (Working)
                    return;

                Working = true;
                eventLock = new ManualResetEvent(true);

                if (jobsTodo == null)
                    throw new ArgumentException("No items in the passed list");

                if (MaxConcurrentWorkers == 0)
                    throw new InvalidOperationException("MaxConcurrentWorkers has not been set.");

                currentWorkers.Clear();

                CurrentConcurrentWorkers = 0;
                CurrentWork = 0;
                CompletedWorks = 0;
            }

            Task.Factory.StartNew(() => InternalStart(jobsTodo));
        }

        /// <summary>
        /// Setting the maximum of the concurrent works
        /// </summary>
        /// <param name="count"></param>
        /// Exceptions:
        ///   System.FieldAccessException: Cannot set the number of maximum concurrent workers while working in progress.
        public void SetMaxConcurrentWorkers(Int32 count)
        {
            if (Working) throw new FieldAccessException("Cannot set the number of maximum concurrent workers while working in progress.");
            {
                lock (objectLock)
                {
                    MaxConcurrentWorkers = count;
                }
            }
        }

        public void StopWorks()
        {
            lock (objectLock)
            {
                Working = false;

                var temp = new List<IWorker>(currentWorkers);

                foreach (var currentWorker in temp)
                    currentWorker.CancelWork();

                currentWorkers.Clear();
            }

        }

        protected virtual void InternalStart(IList<T> jobsTodo)
        {
            while (Working)
            {
                eventLock.WaitOne();

                while (CurrentConcurrentWorkers < MaxConcurrentWorkers && CurrentWork < jobsTodo.Count && Working)
                {
                    var work = workerFactory.CreateWorker(jobsTodo[CurrentWork]);

                    lock (objectLock)
                        currentWorkers.Add(work);

                    work.WorkCompleted += WorkCompleted;
                    work.StartWork();

                    Interlocked.Increment(ref CurrentConcurrentWorkers);
                    Interlocked.Increment(ref CurrentWork);
                }

                eventLock.Reset();

                lock (objectLock)
                {
                    if (CompletedWorks != jobsTodo.Count)
                        continue;

                    OnWorksCompleted(CompletedWorks);
                    Working = false;
                    break;
                }
            }
        }

        protected virtual void WorkCompleted(object sender, EventArgs eventArgs)
        {
            var work = sender as IWorker;

            lock (objectLock)
                currentWorkers.Remove(work);

            if (work != null)
            {
                OnOneWorkCompleted(eventArgs);

                work.WorkCompleted -= WorkCompleted;

                Interlocked.Decrement(ref CurrentConcurrentWorkers);
                Interlocked.Increment(ref CompletedWorks);

                lock (objectLock)
                    if (CurrentConcurrentWorkers < MaxConcurrentWorkers)
                        eventLock.Set();
            }
        }

        protected void OnWorksCompleted(Int32 e)
        {
            var handler = WorksCompleted;
            if (handler != null) handler(this, new CounterEventArg(e));
        }

        protected void OnOneWorkCompleted(EventArgs eventArgs)
        {
            var handler = OneWorkCompleted;
            if (handler != null) 
                handler(this, eventArgs);
        }
    }
}
