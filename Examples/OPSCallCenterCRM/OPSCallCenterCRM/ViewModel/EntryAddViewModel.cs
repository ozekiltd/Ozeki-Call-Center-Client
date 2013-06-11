using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRMAPI;

namespace OPSCallCenterCRM.ViewModel
{
    class EntryAddViewModel : ViewModelBase
    {
        public CrmEntry CurrentCrmEntry { get; set; }

        public RelayCommand Add { get; set; }
        public RelayCommand Cancel { get; set; }

        public EntryAddViewModel()
        {
            CurrentCrmEntry = new CrmEntry();
            InitCommands();
        }

        private void InitCommands()
        {
            Add = new RelayCommand(InitAdd);
            Cancel = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage(MainViewModel.DismissAddWindow)));
        }

        private void InitAdd()
        {
            if (string.IsNullOrEmpty(CurrentCrmEntry.PhoneNumber) || string.IsNullOrEmpty(CurrentCrmEntry.FirstName) || string.IsNullOrEmpty(CurrentCrmEntry.PhoneNumber))
            {
                MessageBox.Show("You have to specify your new contact's phone number, first and last name!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Messenger.Default.Send(new NotificationMessage(CurrentCrmEntry, MainViewModel.EntryAdded));
            Messenger.Default.Send(new NotificationMessage(MainViewModel.DismissAddWindow));
        }
    }
}
