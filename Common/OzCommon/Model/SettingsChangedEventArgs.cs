
using System;
namespace OzCommon.Model
{
    public class SettingsChangedEventArgs<T> : EventArgs
    {
        public SettingsChangedEventArgs(T oldSettings, T newSettings)
        {
            OldSettings = oldSettings;
            NewSettings = newSettings;
        }

        public T OldSettings { get; private set; }
        public T NewSettings { get; private set; }
    }
}
