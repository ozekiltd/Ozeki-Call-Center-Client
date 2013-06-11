using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;
using OPSCallCenterCRMServer.Model;
using OPSCallCenterCRMServer.Model.Database;
using OPSCallCenterCRMServer.Model.Settings;
using OPSCallCenterCRMServer.Model.Wcf;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.ViewModel;

namespace OPSCallCenterCRMServer.ViewModel
{
    class MainViewModel : CommonMainViewModel<CrmEntry>
    {
        public static string EntryAdded = Guid.NewGuid().ToString();
        public static string EntryDeleted = Guid.NewGuid().ToString();
        public static string EntryModified = Guid.NewGuid().ToString();
        public static string CallHistoryEntryAdded = Guid.NewGuid().ToString();
        public static string CallReceived = Guid.NewGuid().ToString();

        public static string ReRegisterDatabaseClient = Guid.NewGuid().ToString();
        public static string ReRegisterServiceHost = Guid.NewGuid().ToString();

        private const int MaxLogsize = 1000;

        private IGenericSettingsRepository<AppPreferences> _settingsRepository;
        private WcfConnectionProperties _wcfConnectionProperties;
        private DatabaseConnectionProperties _databaseConnectionProperties;

        public ObservableCollectionEx<string> Logs { get; set; }
        public RelayCommand ClearLog { get; set; }

        public MainViewModel()
        {
            _settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();

            InitSettings();
            InitViewModelCommands();
            Logs = new ObservableCollectionEx<string>();

            DatabaseStarter.StartDatabase(_settingsRepository.GetSettings().DatabaseConnectionProperties);
            WcfCrmServer.NotificationReceived += WcfCrmServerOnNotificationReceived;
        }

        private void WcfCrmServerOnNotificationReceived(object sender, NotificationEventArgs notificationEventArgs)
        {
            if (Logs.Count >= MaxLogsize)
                Logs.RemoveAt(0);

            Logs.Add(GetLogEntry(notificationEventArgs.Notification));
        }

        private void InitViewModelCommands()
        {
            ClearLog = new RelayCommand(() => Logs.Clear());
        }

        private void InitSettings()
        {
            var settings = _settingsRepository.GetSettings();
            var newSettings = new AppPreferences();

            if (settings != null)
            {
                _wcfConnectionProperties = settings.WcfConnectionProperties;
                _databaseConnectionProperties = settings.DatabaseConnectionProperties;
            }
            else
            {
                _wcfConnectionProperties = WcfConnectionProperties.Default;
                _databaseConnectionProperties = DatabaseConnectionProperties.Default;
            }

            newSettings.WcfConnectionProperties = _wcfConnectionProperties;
            newSettings.DatabaseConnectionProperties = _databaseConnectionProperties;

            _settingsRepository.SetSettings(newSettings);

            Messenger.Default.Send(new NotificationMessage(ReRegisterDatabaseClient));
            Messenger.Default.Send(new NotificationMessage(ReRegisterServiceHost));
        }

        private string GetLogEntry(string message)
        {
            return string.Format("{0} - {1}.", DateTime.Now.ToString(), message);
        }
    }
}
