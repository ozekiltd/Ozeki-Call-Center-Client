using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using OzCommon.ViewModel;
using System.Windows.Media;

namespace OzCommon.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, IWindow
    {
        WaitWindow waitWindow;
        bool connected;
        LoginViewModel viewModel;
        public LoginWindow()
        {
            InitializeComponent();
            viewModel = (LoginViewModel)DataContext;

            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);

            ServerAddress.Focus();
            Closed += LoginWindow_Closed;
        }

        void LoginWindow_Closed(object sender, EventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);
            Closed -= LoginWindow_Closed;
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notificationMessage.Notification == Messages.ShowWaitWindow)
                {
                    if (waitWindow != null)
                        waitWindow.Close();

                    waitWindow = new WaitWindow("Logging in, please wait...", true) {Owner = this};

                    waitWindow.ShowDialog();
                }
                else if (notificationMessage.Notification == Messages.NavigateToMainWindow)
                {
                    if (waitWindow != null)
                        waitWindow.Close();

                    Close();
                }
                else if (notificationMessage.Notification == Messages.DismissWaitWindow)
                {
                    if (waitWindow != null)
                    {
                        waitWindow.Close();
                        waitWindow = null;
                    }
                }
            }));

        }

        public void ShowWindow()
        {
            Activate();
            ShowInTaskbar = true;
            Show();
            WindowState = WindowState.Normal;
            Focus();
        }

        private void LoginCloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void LoginCloseButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            LoginCloseButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void LoginCloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            LoginCloseButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void LoginHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
