namespace OzCommon.Utils.Schedule
{
    public interface IWorkerFactory<T> where T : class 
    {
        IWorker CreateWorker(T work);
    }
}
