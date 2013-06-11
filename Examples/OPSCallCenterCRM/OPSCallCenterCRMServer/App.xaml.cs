using System;
using System.ServiceModel;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;
using OPSCallCenterCRMServer.Model.Database;
using OPSCallCenterCRMServer.Model.Settings;
using OPSCallCenterCRMServer.Model.Wcf;
using OPSCallCenterCRMServer.ViewModel;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.View;
using OzCommon.ViewModel;

namespace OPSCallCenterCRMServer
{
    public partial class App : Application
    {
        private SingletonApp _singletonApp;
        private ServiceHost _serviceHost;

        public App()
        {
            _singletonApp = new SingletonApp("OPSCallCenterCRMServer");

            SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            SimpleIoc.Default.Register<IDatabaseClient<CrmEntry>>(() => new DatabaseClient<CrmEntry>());
            SimpleIoc.Default.Register<IUserInfoSettingsRepository>(() => new UserInfoSettingsRepository());
            SimpleIoc.Default.Register<IClient>(() => new Client());
            SimpleIoc.Default.Register<IGenericSettingsRepository<AppPreferences>>(() => new GenericSettingsRepository<AppPreferences>());
        }

        private void InitServiceHost(WcfConnectionProperties connectionProperties = null)
        {
            if (_serviceHost != null)
                _serviceHost.Abort();

            int max = 5000000;
            var tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Mode = SecurityMode.None;
            
            tcpBinding.OpenTimeout = TimeSpan.FromMinutes(10);
            tcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(1);

            tcpBinding.MaxBufferSize = max;
            tcpBinding.MaxReceivedMessageSize = max;
            tcpBinding.ReaderQuotas.MaxArrayLength = max;

            _serviceHost = new ServiceHost(new WcfCrmServer(), new Uri(connectionProperties == null ? WcfConnectionProperties.Default.ConnectionString : connectionProperties.ConnectionString));
            _serviceHost.AddServiceEndpoint(typeof(ICrmServer), tcpBinding, "CrmServer");
            _serviceHost.Open();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);

            _singletonApp.OnStartup(e);

            MainWindow = new LoginWindow();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);

            DatabaseStarter.StopDatabase();
            base.OnExit(e);
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notificationMessage.Notification == Messages.NavigateToMainWindow)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    Application.Current.MainWindow = mainWindow;
                }
                else if (notificationMessage.Notification == Messages.ShowAboutWindow)
                {
                    var aboutWindow = new AboutWindow("Call Center CRM Server");
                    aboutWindow.ShowDialog();
                }
                else if (notificationMessage.Notification == MainViewModel.ReRegisterDatabaseClient)
                {
                    var settingsRepository = SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();
                    var settings = settingsRepository.GetSettings();

                    SimpleIoc.Default.Unregister<IDatabaseClient<CrmEntry>>();
                    SimpleIoc.Default.Register<IDatabaseClient<CrmEntry>>(() => new DatabaseClient<CrmEntry>(settings.DatabaseConnectionProperties));
                }
                else if (notificationMessage.Notification == MainViewModel.ReRegisterServiceHost)
                {
                    var settingsRepository = SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();
                    var settings = settingsRepository.GetSettings();

                    InitServiceHost(settings.WcfConnectionProperties);
                }
            }));
        }
    }
}
