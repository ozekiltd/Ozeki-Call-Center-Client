using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRM.Model.Settings;
using OPSCallCenterCRM.Model.WCF;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.ViewModel;

namespace OPSCallCenterCRM.ViewModel
{
    public class MainViewModel : CommonMainViewModel<CrmEntry>
    {
        public static string InitRebuildCrmEntries = Guid.NewGuid().ToString();
        public static string InitAddWindow = Guid.NewGuid().ToString();
        public static string DismissAddWindow = Guid.NewGuid().ToString();
        public static string ShowWcfWarningWindow = Guid.NewGuid().ToString();
        public static string EntryAdded = Guid.NewGuid().ToString();
        public static string CallReceived = Guid.NewGuid().ToString();

        private CrmEntry _selectedCrmEntry;
        private IGenericSettingsRepository<AppPreferences> _settingsRepository;
        private WcfCrmClient _wcfCrmClient;
        private object _lockObj;
        private ObservableCollectionEx<CrmEntry> _internalCrmEntries;
        private ICollectionView _crmEntries;
        private string _filter;

        public RelayCommand AddNote { get; set; }
        public RelayCommand RemoveNote { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand ModifySelectedCrmEntry { get; set; }
        public RelayCommand RevertSelectedCrmEntry { get; set; }

        public CrmNote SelectedCrmEntryNote { get; set; }
        public CrmNote NewCrmEntryNote { get; set; }

        public ICollectionView CrmEntries
        {
            get { return _crmEntries; }
            set
            {
                _crmEntries = value;
                _crmEntries.Filter = FilterItem;

                RaisePropertyChanged(() => CrmEntries);
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                CrmEntries.Refresh();
            }
        }

        public CrmEntry SelectedCrmEntry
        {
            get { return _selectedCrmEntry; }
            set
            {
                _selectedCrmEntry = value != null ? new CrmEntry(value) : null;
                RaisePropertyChanged("SelectedCrmEntry");
                RaisePropertyChanged("IsControlVisible");
            }
        }

        public MainViewModel()
        {
            _settingsRepository = SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();
            _lockObj = new object();

            _wcfCrmClient = SimpleIoc.Default.GetInstance<WcfCrmClient>();
            _wcfCrmClient.CrmServerCallHistoryEntryAdded += WcfCrmClientOnCrmServerCallHistoryEntryAdded;
            _wcfCrmClient.CrmServerCallReceived += WcfCrmClientOnCrmServerCallReceived;
            _wcfCrmClient.CrmServerEntryAdded += WcfCrmClientOnCrmServerEntryAdded;
            _wcfCrmClient.CrmServerEntryDeleted += WcfCrmClientOnCrmServerEntryDeleted;
            _wcfCrmClient.CrmServerEntryModified += WcfCrmClientOnCrmServerEntryModified;

            _internalCrmEntries = new ObservableCollectionEx<CrmEntry>();
            CrmEntries = CollectionViewSource.GetDefaultView(_internalCrmEntries);
            NewCrmEntryNote = new CrmNote();

            InitMessenger();
            InitSettings();
            InitViewCommands();

            _wcfCrmClient.Connect();
            RebuildCrmEntries();
        }

        #region InitCommands

        private void InitViewCommands()
        {
            AddNote = new RelayCommand(() =>
                                           {
                                               if (SelectedCrmEntry != null)
                                               {
                                                   var selectedEntry = _internalCrmEntries.FirstOrDefault(entry => entry.ID.Equals(SelectedCrmEntry.ID));
                                                   if (selectedEntry != null)
                                                   {
                                                       NewCrmEntryNote.OriginatorClient = _settingsRepository.GetSettings().ClientCredential.UserName;
                                                       selectedEntry.Notes.Add(NewCrmEntryNote);
                                                   }

                                                   _wcfCrmClient.ModifyEntry(selectedEntry);
                                               }
                                           });

            RemoveNote = new RelayCommand(() =>
                                              {
                                                  if (MessageBox.Show("Are you sure you want to delete the selected note?", "Confirm delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                                      return;

                                                  if (SelectedCrmEntry != null && SelectedCrmEntryNote != null)
                                                  {
                                                      var selectedEntry = _internalCrmEntries.FirstOrDefault(entry => entry.ID.Equals(SelectedCrmEntry.ID));
                                                      if (selectedEntry != null)
                                                          selectedEntry.Notes.Remove(SelectedCrmEntryNote);

                                                      _wcfCrmClient.ModifyEntry(selectedEntry);
                                                  }
                                              });

            ModifySelectedCrmEntry = new RelayCommand(() =>
                                                          {
                                                              if (SelectedCrmEntry != null)
                                                              {
                                                                  if (string.IsNullOrEmpty(SelectedCrmEntry.PhoneNumber))
                                                                  {
                                                                      MessageBox.Show("Your contact must have a phone number!", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                                                      return;
                                                                  }

                                                                  var selectedEntry = _internalCrmEntries.FirstOrDefault(entry => entry.ID.Equals(SelectedCrmEntry.ID));
                                                                  if (selectedEntry != null)
                                                                      selectedEntry = SelectedCrmEntry;

                                                                  _wcfCrmClient.ModifyEntry(selectedEntry);
                                                              }
                                                          });

            RevertSelectedCrmEntry = new RelayCommand(() =>
                                                          {
                                                              if (SelectedCrmEntry != null)
                                                              {
                                                                  var selectedEntry = _internalCrmEntries.FirstOrDefault(entry => entry.ID.Equals(SelectedCrmEntry.ID));
                                                                  if (selectedEntry != null)
                                                                      SelectedCrmEntry = selectedEntry;
                                                              }
                                                          });

            Delete = new RelayCommand(() =>
                                          {
                                              if (MessageBox.Show("Are you sure you want to delete this entry?", "Confirm delete", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                                  return;

                                              if (SelectedCrmEntry != null)
                                              {
                                                  var selectedEntry = _internalCrmEntries.FirstOrDefault(entry => entry.ID.Equals(SelectedCrmEntry.ID));
                                                  if (selectedEntry != null)
                                                      _internalCrmEntries.Remove(selectedEntry);

                                                  _wcfCrmClient.DeleteEntry(selectedEntry);
                                              }
                                          });
        }

        #endregion

        private void InitMessenger()
        {
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);
        }

        private void InitSettings()
        {
            var settings = _settingsRepository.GetSettings();

            if (settings == null)
            {
                var preferences = new AppPreferences() { ConnectionProperties = WcfConnectionProperties.Default };
                _settingsRepository.SetSettings(preferences);
            }
        }

        private void RefreshGrid()
        {
            CrmEntries.Refresh();
            RaisePropertyChanged("CrmEntries");
        }

        private void RebuildCrmEntries()
        {
            try
            {
                var entries = _wcfCrmClient.GetAllCrmEntries();
                _internalCrmEntries = new ObservableCollectionEx<CrmEntry>();
                _internalCrmEntries.AddItems(entries);

                CrmEntries = CollectionViewSource.GetDefaultView(_internalCrmEntries);
                CrmEntries.Filter = FilterItem;

                if (_internalCrmEntries.Count > 0)
                    SelectedCrmEntry = _internalCrmEntries.First();

                RaisePropertyChanged("CrmEntries");
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage(ex.Message, ShowWcfWarningWindow));
            }
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == InitRebuildCrmEntries)
                RebuildCrmEntries();
            else if (notificationMessage.Notification == EntryAdded)
            {
                var newEntry = notificationMessage.Sender as CrmEntry;
                if (newEntry != null)
                {
                    _wcfCrmClient.AddEntry(newEntry);
                }
            }
        }

        private void WcfCrmClientOnCrmServerEntryModified(object sender, CrmEntry crmEntry)
        {
            lock (_lockObj)
            {
                var entry = _internalCrmEntries.FirstOrDefault(en => en.ID == crmEntry.ID);

                if (entry != null)
                    _internalCrmEntries.Remove(entry);

                _internalCrmEntries.Add(crmEntry);

                if (entry != null)
                    if (SelectedCrmEntry == null || SelectedCrmEntry.ID == entry.ID)
                        SelectedCrmEntry = crmEntry;
            }

            RefreshGrid();
        }

        private void WcfCrmClientOnCrmServerEntryDeleted(object sender, CrmEntry crmEntry)
        {
            lock (_lockObj)
            {
                _internalCrmEntries.Remove(crmEntry);
                if (SelectedCrmEntry == null || SelectedCrmEntry.ID == crmEntry.ID)
                {
                    if (_internalCrmEntries.Count > 0)
                        SelectedCrmEntry = crmEntry;
                }
            }

            RaisePropertyChanged("CrmEntries");
        }

        private void WcfCrmClientOnCrmServerEntryAdded(object sender, CrmEntry crmEntry)
        {
            lock (_lockObj)
                _internalCrmEntries.Add(crmEntry);

            RaisePropertyChanged("CrmEntries");
        }

        private void WcfCrmClientOnCrmServerCallReceived(object sender, CallSessionInfo callSessionInfo)
        {
            var entry = _internalCrmEntries.FirstOrDefault(en => en.PhoneNumber.Equals(callSessionInfo.Caller));

            if (entry == null)
                entry = new CrmEntry { BusinessName = "Unknown", PhoneNumber = callSessionInfo.Caller, FirstName = "Unknown" };

            Messenger.Default.Send(new NotificationMessage(entry, CallReceived));
        }

        private void WcfCrmClientOnCrmServerCallHistoryEntryAdded(object sender, CallHistoryEntry callHistoryEntry)
        {
            lock (_lockObj)
            {
                var affectedEntries = _internalCrmEntries.Where(entry => entry.PhoneNumber.Equals(callHistoryEntry.Callee) || entry.PhoneNumber.Equals(callHistoryEntry.Caller));
                foreach (var affectedEntry in affectedEntries)
                    affectedEntry.CallHistoryEntries.Add(callHistoryEntry);
            }

            RaisePropertyChanged("CrmEntries");
        }

        private bool FilterItem(object obj)
        {
            var item = (CrmEntry)obj;

            if (string.IsNullOrEmpty(Filter))
                return true;

            var filterStr = Filter.ToLower();

            return
                (!string.IsNullOrEmpty(item.FirstName) && item.FirstName.ToLower().Contains(filterStr)) ||
                (!string.IsNullOrEmpty(item.LastName) && item.LastName.ToLower().Contains(filterStr)) ||
                (!string.IsNullOrEmpty(item.PhoneNumber) && item.PhoneNumber.Contains(filterStr)) ||
                (!string.IsNullOrEmpty(item.Email) && item.Email.ToLower().Contains(filterStr)) ||
                (!string.IsNullOrEmpty(item.BusinessName) && item.BusinessName.ToLower().Contains(filterStr)) ||
                ((item.Notes != null && item.Notes.Count != 0) && item.Notes.Any(note => note.Note.ToLower().Contains(filterStr)));
        }
    }
}
