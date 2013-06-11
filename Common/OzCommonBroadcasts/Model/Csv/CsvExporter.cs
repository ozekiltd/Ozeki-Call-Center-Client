using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OzCommon.Utils;

namespace OzCommonBroadcasts.Model.Csv
{
    public class CsvExporter<T> : ICsvExporter<T> where T : class
    {
        #region FastEvents
        readonly FastSmartWeakEvent<EventHandler<CounterEventArg>> allItemLoadedFastSmartWeakEvent;

        public event EventHandler<CounterEventArg> AllItemSaved
        {
            add { allItemLoadedFastSmartWeakEvent.Add(value); }
            remove { allItemLoadedFastSmartWeakEvent.Remove(value); }
        }
        #endregion

        object sync;
        public bool IsSaving { get; private set; }

        public CsvExporter()
        {
            sync = new object();
            allItemLoadedFastSmartWeakEvent = new FastSmartWeakEvent<EventHandler<CounterEventArg>>();
        }

        string Export(List<T> objects, bool includeHeaderLine = true)
        {
            var sb = new StringBuilder();

            try
            {
                var propertyInfos = typeof(T).GetProperties();

                if (includeHeaderLine)
                {
                    foreach (var propertyInfo in propertyInfos)
                    {
                        if (CanSkipProperty(propertyInfo)) continue;

                        sb.Append(propertyInfo.Name).Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1).AppendLine();
                }

                foreach (var obj in objects)
                {
                    if (!IsSaving)
                        break;

                    foreach (var propertyInfo in propertyInfos)
                    {
                        if (CanSkipProperty(propertyInfo)) continue;

                        sb.Append(MakeValueCsvFriendly(propertyInfo.GetValue(obj, null)))
                          .Append(",");
                    }

                    sb.Remove(sb.Length - 1, 1).AppendLine();
                }

            }
            catch (Exception)
            {
                OnAllItemSaved(-1);
                return "";
            }

            OnAllItemSaved(1);
            return sb.ToString();
          
        }

        static bool CanSkipProperty(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(true);

            //If have ExportIgnore then do not write out to the file
            var skipThis =
                ( from Attribute attribute in attributes select attribute.GetType() ).Any(
                    attributeType => attributeType == typeof (ExportIgnorePropertyAttribute));

            return skipThis;
        }

        public void ExportToFile(List<T> objects, string path)
        {
            Task.Factory.StartNew(() =>
            {
                lock (sync)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        OnAllItemSaved(-1);
                        return;
                    }

                    IsSaving = true;
                    File.WriteAllText(path, Export(objects));

                    IsSaving = false;
                }
            });
        }

        public void Cancel()
        {
            IsSaving = false;
        }

        public void OnAllItemSaved(Int32 exportedRow)
        {
            allItemLoadedFastSmartWeakEvent.Raise(this, new CounterEventArg(exportedRow));
        }

        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is Nullable && ((INullable)value).IsNull) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            var output = value.ToString();

            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;

        }
    }
}

