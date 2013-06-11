using System.Windows;

namespace OzCommon.Utils.DialogService
{
    public interface IDialogService
    {
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton buttons, MessageBoxImage icon);
        string ShowOpenFileDialog(string filter);
        string ShowSaveFileDialog(string defaultExt, string filter);
    }
}
