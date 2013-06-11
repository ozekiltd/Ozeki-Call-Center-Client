using System;

namespace OzCommon.Model
{
    public interface IGenericSettingsRepository<T> where T : class
    {
        event EventHandler<SettingsChangedEventArgs<T>> SettingsChanged;
        T GetSettings();
        void SetSettings(T obj);
    }
}
