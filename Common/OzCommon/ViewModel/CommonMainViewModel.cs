using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace OzCommon.ViewModel
{
    public class CommonMainViewModel<T> : ViewModelBase
    {
        public RelayCommand About { get; protected set; }
        public RelayCommand HomePage { get; protected set; }

        public CommonMainViewModel()
        {
            InitCommands();
        }

        public void InitCommands()
        {
            HomePage = new RelayCommand(() => Process.Start("http://www.ozekiphone.com/"));

            About = new RelayCommand(() =>
                                         {
                                             Messenger.Default.Send(new NotificationMessage(this,
                                                                                            Messages.ShowAboutWindow));
                                         });
        }
    }
}