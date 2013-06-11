using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OzCommon.Utils;

namespace OzCommonBroadcasts.Model.Csv
{
    public class CsvImporter<T> : ICsvImporter<T> where T : EventArgs
    {
        #region FastEvents
        readonly FastSmartWeakEvent<EventHandler<T>> itemLoadedFastSmartWeakEvent;
        public event EventHandler<T> ItemLoaded
        {
            add { itemLoadedFastSmartWeakEvent.Add(value); }
            remove { itemLoadedFastSmartWeakEvent.Remove(value); }
        }

        readonly FastSmartWeakEvent<EventHandler<CounterEventArg>> allItemLoadedFastSmartWeakEvent;
        public event EventHandler<CounterEventArg> AllItemLoaded
        {
            add { allItemLoadedFastSmartWeakEvent.Add(value); }
            remove { allItemLoadedFastSmartWeakEvent.Remove(value); }
        }
        #endregion

        public bool IsLoading { get; private set; }
        readonly object sync;

        public CsvImporter()
        {
            allItemLoadedFastSmartWeakEvent = new FastSmartWeakEvent <EventHandler <CounterEventArg>>();
            itemLoadedFastSmartWeakEvent = new FastSmartWeakEvent<EventHandler<T>>();
            sync = new object();
        }

        public void LoadFile(string path)
        {
            lock (sync)
            {
                Task.Factory.StartNew(() =>
                    {
                        IsLoading = true;
                        if (string.IsNullOrEmpty(path))
                        {
                            OnAllItemLoaded(this,-1);
                            return;
                        }

                        var importedRow = 0;
                        try
                        {
                            using (var fileReader = new StreamReader(path))
                            {
                                var headerLine = fileReader.ReadLine();
                                var csvheaders = new Dictionary<int, string>();

                                if (headerLine == null)
                                {
                                    OnAllItemLoaded(this, -1);
                                    return;
                                }

                                var headerValues = headerLine.Split(',');
                                for (var i = 0; i < headerValues.Length; i++)
                                    csvheaders.Add(i, headerValues[i]);

                                string bodyLine;
                                while ((bodyLine = fileReader.ReadLine()) != null)
                                {
                                    if (!IsLoading)
                                        break;

                                    var genericBodyValue = Activator.CreateInstance<T>();
                                    var bodyValues = bodyLine.Split(',');

                                    for (var i = 0; i < bodyValues.Length; i++)
                                    {
                                        var currentValue = i;

                                        if (csvheaders.ContainsKey(i))
                                        {
                                            var prop = genericBodyValue.GetType().GetProperty(csvheaders[currentValue]);
                                            prop.SetValue(genericBodyValue, Convert.ChangeType(bodyValues[i], prop.PropertyType), null);
                                        }
                                    }
                                    importedRow++;
                                    OnItemLoaded(this, genericBodyValue);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            OnAllItemLoaded(this, -1);
                        }

                        IsLoading = false;

                        OnAllItemLoaded(this, importedRow);
                    }); 
            }
        }

        public void Cancel()
        {
            IsLoading = false;
        }

        public void OnItemLoaded(object sender,T e)
        {
            itemLoadedFastSmartWeakEvent.Raise(sender, e);
        }

        public void OnAllItemLoaded(object sender, Int32 importedRow)
        {
            allItemLoadedFastSmartWeakEvent.Raise(sender, new CounterEventArg(importedRow));
        }
    }


}
