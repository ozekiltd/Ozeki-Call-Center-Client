using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OzCommon.Utils;
using OzCommonBroadcasts.Model;

namespace OzCommonBroadcasts.Utils
{

    public class ClipboardHelper<T> where T : EventArgs 
    {
        #region FastEvents
        readonly FastSmartWeakEvent<EventHandler<T>> itemLoadedFastSmartWeakEvent;
        readonly FastSmartWeakEvent<EventHandler> allItemLoadedFastSmartWeakEvent;

        public event EventHandler<T> ItemLoaded
        {
            add { itemLoadedFastSmartWeakEvent.Add(value); }
            remove { itemLoadedFastSmartWeakEvent.Remove(value); }
        }

        public event EventHandler AllItemLoaded
        {
            add { allItemLoadedFastSmartWeakEvent.Add(value); }
            remove { allItemLoadedFastSmartWeakEvent.Remove(value); }
        } 
        #endregion

        public delegate string[] ParseFormat(string value);

        public ClipboardHelper()
        {
            itemLoadedFastSmartWeakEvent = new FastSmartWeakEvent<EventHandler<T>>();
            allItemLoadedFastSmartWeakEvent = new FastSmartWeakEvent <EventHandler>();
        }

        public void PasteData()
        {
            var rowData = ParseClipboardData();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var csvheaders = new Dictionary<int, string>();

                    for (var i = 0; i < rowData[0].Length; i++)
                        csvheaders.Add(i, rowData[0][i]);

                    //Removing the headers
                    rowData.RemoveAt(0);

                    foreach (string[] t1 in rowData)
                    {
                        var genericBodyValue = Activator.CreateInstance<T>();

                        for (var i = 0; i < t1.Length; i++)
                        {
                            var currentValue = i;

                            if (csvheaders.ContainsKey(i))
                            {
                                var prop = genericBodyValue.GetType().GetProperty(csvheaders[currentValue]);

                                if (prop.PropertyType.IsEnum || !prop.PropertyType.IsSerializable)
                                    continue;

                                prop.SetValue(genericBodyValue, Convert.ChangeType(t1[i], prop.PropertyType), null);
                            }
                        }
                        OnItemLoaded(this, genericBodyValue);
                    }
                }
                catch (Exception)
                {
                    OnAllItemLoaded(this, -1);
                }

                OnAllItemLoaded(this, 1);
            });
        }

        public void OnItemLoaded(object sender, T e)
        {
            itemLoadedFastSmartWeakEvent.Raise(sender,e);
        }

        public void OnAllItemLoaded(object sender, Int32 importedRow)
        {
            allItemLoadedFastSmartWeakEvent.Raise(sender, new CounterEventArg(importedRow));
        }

        private static List<string[]> ParseClipboardData()
        {
            List<string[]> clipboardData = null;
            object clipboardRawData = null;
            ParseFormat parseFormat = null;

            // get the data and set the parsing method based on the format
            // currently works with CSV and Text DataFormats            
            IDataObject dataObj = Clipboard.GetDataObject();
            if ((clipboardRawData = dataObj.GetData(DataFormats.CommaSeparatedValue)) != null)
            {
                parseFormat = ParseCsvFormat;
            }
            else if ((clipboardRawData = dataObj.GetData(DataFormats.Text)) != null)
            {
                parseFormat = ParseTextFormat;
            }

            if (parseFormat != null)
            {
                var rawDataStr = clipboardRawData as string;

                if (rawDataStr == null && clipboardRawData is MemoryStream)
                {
                    // cannot convert to a string so try a MemoryStream
                    var ms = clipboardRawData as MemoryStream;
                    var sr = new StreamReader(ms);
                    rawDataStr = sr.ReadToEnd();
                }
                Debug.Assert(rawDataStr != null, string.Format("clipboardRawData: {0}, could not be converted to a string or memorystream.", clipboardRawData));

                string[] rows = rawDataStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (rows != null && rows.Length > 0)
                {
                    clipboardData = rows.Select(row => parseFormat(row)).ToList();
                }
                else
                {
                    Debug.WriteLine("unable to parse row data.  possibly null or contains zero rows.");
                }
            }

            return clipboardData;
        }

        private static string[] ParseCsvFormat(string value)
        {
            return ParseCsvOrTextFormat(value, true);
        }

        private static string[] ParseTextFormat(string value)
        {
            return ParseCsvOrTextFormat(value, false);
        }

        public void CopyString(IList param)
        {
            var sb = new StringBuilder();

            foreach (var t1 in param)
            {
                var s = t1.GetType().GetProperties();
                foreach (var propertyInfo in s)
                {
                    if (CanSkipProperty(propertyInfo)) continue;

                    sb.Append(propertyInfo.Name).Append("\t");
                }

                sb.AppendLine();

                break;
            }

            foreach (var t1 in param)
            {
                var s = t1.GetType().GetProperties();

                foreach (var propertyInfo in s)
                {
                    if (CanSkipProperty(propertyInfo)) continue;

                    sb.Append(propertyInfo.GetValue(t1, null)).Append("\t");
                }
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        static bool CanSkipProperty(PropertyInfo propertyInfo)
        {
            var attributes = propertyInfo.GetCustomAttributes(true);

            //If have ExportIgnore then do not write out to the file
            var skipThis =
                (from Attribute attribute in attributes select attribute.GetType()).Any(
                    attributeType => attributeType == typeof(ExportIgnorePropertyAttribute));

            return skipThis;
        }


        private static string[] ParseCsvOrTextFormat(string value, bool isCSV)
        {
            var outputList = new List<string>();

            char separator = isCSV ? ',' : '\t';
            var startIndex = 0;
            var endIndex = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (ch == separator)
                {
                    outputList.Add(value.Substring(startIndex, endIndex - startIndex));

                    startIndex = endIndex + 1;
                    endIndex = startIndex;
                }
                else if (ch == '\"' && isCSV)
                {
                    // skip until the ending quotes
                    i++;
                    if (i >= value.Length)
                    {
                        throw new FormatException(string.Format("value: {0} had a format exception", value));
                    }
                    char tempCh = value[i];
                    while (tempCh != '\"' && i < value.Length)
                        i++;

                    endIndex = i;
                }
                else if (i + 1 == value.Length)
                {
                    // add the last value
                    outputList.Add(value.Substring(startIndex));
                    break;
                }
                else
                {
                    endIndex++;
                }
            }

            return outputList.ToArray();
        }
    }
}
