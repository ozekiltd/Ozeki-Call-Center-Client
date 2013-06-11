using System;

namespace OzCommonBroadcasts.Model
{
    public interface ICompletedWork
    {
        Boolean IsCompleted { get; set; }
        Boolean IsValid { get; }
    }
}
