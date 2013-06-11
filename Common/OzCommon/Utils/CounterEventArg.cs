using System;

namespace OzCommon.Utils
{
    public class CounterEventArg : EventArgs
    {
        public Int32 Count { get; private set; }
        public CounterEventArg(Int32 count)
        {
            Count = count;
        }
    }
}
