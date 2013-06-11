using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace OzCommon.Model
{
    public class GenericSettingsRepository<T> : IGenericSettingsRepository<T> where T : class
    {
        private T Settings { get; set; }

        public string AppPath { get; private set; }
        public string AppConfigPath { get; private set; }
        public string AppConfigFile { get; private set; }

        public GenericSettingsRepository(string applicationFolder = null, string fileName = "settings.xml")
        {
            if (applicationFolder == null)
                applicationFolder = Assembly.GetEntryAssembly().GetName().Name;
            AppPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppConfigPath = Path.Combine(AppPath, "Ozeki", applicationFolder);
            AppConfigFile = Path.Combine(AppConfigPath, fileName);

            ReadSettings();
        }

        public event EventHandler<SettingsChangedEventArgs<T>> SettingsChanged;

        public T GetSettings()
        {
            return Settings;
        }

        public void SetSettings(T obj)
        {
            OnSettingsChanged(obj);
            Settings = obj;
            WriteSettings();
        }

        private void OnSettingsChanged(T obj)
        {
            var handler = SettingsChanged;

            if(handler != null)
                handler(this, new SettingsChangedEventArgs<T>(Settings, obj));
        }

        private void WriteSettings()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                if (Settings == null)
                {
                    if (File.Exists(AppConfigFile))
                        File.Delete(AppConfigFile);
                    return;
                }

                if (!Directory.Exists(AppConfigPath))
                    Directory.CreateDirectory(AppConfigPath);

                using (var fs = File.Create(AppConfigFile))
                    serializer.Serialize(fs, Settings);
            }
            catch (Exception) { }
        }

        private void ReadSettings()
        {
            try
            {
                var serializer = new XmlSerializer(typeof (T));

                if (!Directory.Exists(AppConfigPath))
                    return;

                if (!File.Exists(AppConfigFile))
                    return;

                using (var fs = File.OpenRead(AppConfigFile))
                    Settings = (T) serializer.Deserialize(fs);
            }
            catch (Exception)
            {
                if (File.Exists(AppConfigFile))
                    File.Delete(AppConfigFile);
            }
            }
    }
}
