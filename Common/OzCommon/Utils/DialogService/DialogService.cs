using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using OzCommon.View;
using System.Windows.Threading;

namespace OzCommon.Utils.DialogService
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton buttons, MessageBoxImage icon)
        {
            var result = MessageBoxResult.No;
           
            Application.Current.Dispatcher.Invoke(new Action(() =>
                                                    {

                                                        var activeMainwindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                                                        result = MessageBox.Show(activeMainwindow ?? Application.Current.MainWindow,
                                                                                 messageBoxText, caption, buttons, icon);
                                                    }));
            return result;
        }

        public string ShowOpenFileDialog(string filter)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Filter = filter,
                    Title = "Opening file"
                };

                bool? result = dlg.ShowDialog();
                return result == true ? dlg.FileName : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ShowSaveFileDialog(string defaultExt, string filter)
        {
            var saveFileDialog1 = new SaveFileDialog
            {
                DefaultExt = defaultExt,
                Filter = filter,
                Title = "Save to file"
            };
            saveFileDialog1.ShowDialog();

            var filePath = saveFileDialog1.FileName == "" ? null : saveFileDialog1.FileName;
            return filePath;
        }
    }
}
