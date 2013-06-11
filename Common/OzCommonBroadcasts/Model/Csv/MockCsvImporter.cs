using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OzCommonBroadcasts.Model.Csv
{
    public class MockCsvImporter<T> : ICsvImporter<T>
                                  where T : EventArgs
    {
        public void LoadFile(string path)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          IsLoading = true;
                                          {
                                              var headerLine = "Email, Id, Name, State";
                                              var csvheaders = new Dictionary<int, string>();

                                              if (headerLine == null)
                                                  return;

                                              var headerValues = headerLine.Split(',');
                                              for (var j = 0; j < headerValues.Length; j++)
                                                  csvheaders.Add(j, headerValues[j]);

                                              string bodyLine;
                                              for (int i = 0; i < 100; i++)
                                              {
                                                  Thread.Sleep(100);
                                                  bodyLine = "Email1, 1, Name1, State1";
                                                  var genericBodyValue = Activator.CreateInstance<T>();
                                                  var bodyValues = bodyLine.Split(',');

                                                  for (var j = 0; j < bodyValues.Length; j++)
                                                  {
                                                      var currentValue = j;

                                                      //Needs Refact
                                                      //TODO KREKKON
                                                      if (csvheaders.ContainsKey(j))
                                                          genericBodyValue.GetType().GetProperty(csvheaders[currentValue]).SetValue(
                                                              genericBodyValue, bodyValues[j], null);
                                                  }

                                                  OnItemLoaded(genericBodyValue);
                                              }
                                          }
                                          IsLoading = false;
                                      });
        }

        private void OnItemLoaded(T customerEntry)
        {
            var handler = ItemLoaded;

            if (handler != null)
                handler(this, customerEntry);
        }

        public void Cancel()
        {

        }

        public bool IsLoading { get; private set; }

        public event EventHandler<T> ItemLoaded;
        public event EventHandler<EntryCountEventArgs> AllItemLoaded;
    }
}
