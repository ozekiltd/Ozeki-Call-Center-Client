using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRM.Model.Settings;
using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;
using OzCommon.Model;
using OzCommon.ViewModel;

namespace OPSCallCenterCRM.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public RelayCommand Login { get; set; }
        public UserInfo UserInfo { get; private set; }
        public bool RememberMe { get; set; }

        private IUserInfoSettingsRepository _userSettingsRepository;
        private IGenericSettingsRepository<AppPreferences> _settingsRepository;

        public LoginViewModel()
        {
            _userSettingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IUserInfoSettingsRepository>();
            _settingsRepository = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IGenericSettingsRepository<AppPreferences>>();

            UserInfo = _userSettingsRepository.GetSettings();
            var settings = _settingsRepository.GetSettings();

            if (UserInfo == null)
            {
                UserInfo = new UserInfo();
                if (settings == null)
                    UserInfo.ServerAddress = string.Format("{0}:{1}", WcfConnectionProperties.Default.Address,
                                                           WcfConnectionProperties.Default.Port);
                else
                    UserInfo.ServerAddress = string.Format("{0}:{1}", settings.ConnectionProperties.Address,
                                                           settings.ConnectionProperties.Port);
            }
            else
                RememberMe = true;

            InitCommands();
            InitView();
        }

        private void InitView()
        {
            if (string.IsNullOrEmpty(UserInfo.ServerAddress))
            {
                UserInfo.ServerAddress = string.Format("{0}:{1}", WcfConnectionProperties.Default.Address, WcfConnectionProperties.Default.Port);
            }
        }

        private void InitCommands()
        {
            Login = new RelayCommand(() =>
                                         {
                                             Messenger.Default.Send(new NotificationMessage(Messages.ShowWaitWindow));

                                             _userSettingsRepository.SetSettings(RememberMe ? UserInfo : null);

                                             var newSettings = new AppPreferences();
                                             var settings = _settingsRepository.GetSettings();

                                             if (settings == null)
                                             {
                                                 newSettings.ClientCredential = new ClientCredential { PhoneNumber = UserInfo.Password, UserName = UserInfo.Username };
                                                 newSettings.ConnectionProperties = WcfConnectionProperties.Default;
                                             }

                                             if (!UserInfo.ServerAddress.Contains(":"))
                                                 UserInfo.ServerAddress += string.Format(":{0}", WcfConnectionProperties.Default.Port);

                                             var addressParts = UserInfo.ServerAddress.Split(':');
                                             int port;

                                             if (int.TryParse(addressParts[1], out port))
                                             {
                                                 newSettings.ConnectionProperties.Address = addressParts[0];
                                                 newSettings.ConnectionProperties.Port = port;
                                             }

                                             newSettings.ClientCredential = new ClientCredential();
                                             newSettings.ClientCredential.PhoneNumber = UserInfo.Password;
                                             newSettings.ClientCredential.UserName = UserInfo.Username;

                                             _settingsRepository.SetSettings(newSettings);

                                             Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));
                                             Messenger.Default.Send(new NotificationMessage(Messages.NavigateToMainWindow));

                                         }, () => UserInfo.IsValid);
        }
    }
}
