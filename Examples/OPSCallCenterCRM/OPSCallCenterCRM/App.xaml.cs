using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRM.Model.Settings;
using OPSCallCenterCRM.Model.WCF;
using OPSCallCenterCRM.View;
using OPSCallCenterCRM.ViewModel;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;
using OzCommon.Model;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.View;
using OzCommon.ViewModel;
using LoginWindow = OPSCallCenterCRM.View.LoginWindow;

namespace OPSCallCenterCRM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SingletonApp _singletonApp;

        public App()
        {
            _singletonApp = new SingletonApp("OPSCallCenterCRM");

            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IGenericSettingsRepository<AppPreferences>>(() => new SettingsRepository());
            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IUserInfoSettingsRepository>(() => new UserInfoSettingsRepository());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);

            base.OnStartup(e);

            _singletonApp.OnStartup(e);

            MainWindow = new LoginWindow();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);

            var client = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<WcfCrmClient>();
            var settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();

            client.Disconnect(settingsRepository.GetSettings().ClientCredential);
            base.OnExit(e);
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notificationMessage.Notification == Messages.NavigateToMainWindow)
                {
                    var settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();
                    var settings = settingsRepository.GetSettings();

                    try
                    {
                        GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register(() => new WcfCrmClient(settings.ConnectionProperties, settings.ClientCredential));
                    }
                    catch (Exception)
                    { }

                    var mainWindow = new MainWindow();

                    mainWindow.Title = string.Format("{0} - {1} ({2})", "Call Center CRM", settings.ClientCredential.UserName, settings.ClientCredential.PhoneNumber);
                    mainWindow.Show();

                    Application.Current.MainWindow = mainWindow;
                }
                else if (notificationMessage.Notification == Messages.ShowAboutWindow)
                {
                    var aboutWindow = new AboutWindow("Call Center CRM");
                    aboutWindow.ShowDialog();
                }
            }));
        }
    }
}
