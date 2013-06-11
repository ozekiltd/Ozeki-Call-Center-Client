using System;

namespace OPSIVRSystem.Utils
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }
        public GenericEventArgs(T item)
        {
            Item = item;
        }
    }
}
