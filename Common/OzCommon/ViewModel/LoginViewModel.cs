using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPSSDK;
using OzCommon.Model;
using OzCommon.Utils.DialogService;

namespace OzCommon.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public RelayCommand Login { get; private set; }
        public UserInfo UserInfo { get; private set; }
        public bool RememberMe { get; set; }

        private readonly IUserInfoSettingsRepository settingsRepository;
        private readonly IDialogService dialogService;
        private IClient client;

        public LoginViewModel()
        {
            settingsRepository = SimpleIoc.Default.GetInstance<IUserInfoSettingsRepository>();
            dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            client = SimpleIoc.Default.GetInstance<IClient>();
            client.ErrorOccurred += ClientOnErrorOccurred;

            UserInfo = settingsRepository.GetSettings();

            if (UserInfo == null)
                UserInfo = new UserInfo();
            else
                RememberMe = true;
                Login = new RelayCommand(() =>
                   {
                       Messenger.Default.Send(new NotificationMessage(Messages.ShowWaitWindow));

                       client.LoginAsync(UserInfo.ServerAddress, UserInfo.Username, UserInfo.Password, (success) =>
                                                                                                           {
                                                                                                               client.ErrorOccurred -= ClientOnErrorOccurred;

                                                                                                               Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));

                                                                                                               if (success)
                                                                                                               {
                                                                                                                   settingsRepository.SetSettings(RememberMe ? UserInfo : null);
                                                                                                                   Messenger.Default.Send(new NotificationMessage(Messages.NavigateToMainWindow));
                                                                                                               }
                                                                                                               else
                                                                                                               {
                                                                                                                   dialogService.ShowMessageBox("Authentication error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                                                                               }
                                                                                                           });

                   }, () => UserInfo.IsValid);
        }

        void ClientOnErrorOccurred(object sender, ErrorInfo errorInfo)
        {
            Messenger.Default.Send(new NotificationMessage(Messages.DismissWaitWindow));

            switch (errorInfo.Type)
            {
                case ErrorType.ConnectionFailure:
                                    dialogService.ShowMessageBox("Network connection error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                case ErrorType.VersionMismatch:
                                    dialogService.ShowMessageBox("Version mismatch error", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
                default:
                                    dialogService.ShowMessageBox(errorInfo.Message, "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    break;
            }
        }
    }
}
