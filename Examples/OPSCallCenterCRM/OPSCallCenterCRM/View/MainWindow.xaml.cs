using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using OPSCallCenterCRM.ViewModel;
using OPSCallCenterCRMAPI;
using OzCommon.View;
using OzCommon.ViewModel;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace OPSCallCenterCRM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EntryAddWindow _entryAddWindow;

        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);
            MainWindow_OnStateChanged(null, null);
        }

        public void ShowWindow()
        {
            Activate();
            ShowInTaskbar = true;
            Show();
            WindowState = WindowState.Normal;
            Focus();
        }

        void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddEntry_OnClick(object sender, RoutedEventArgs e)
        {
            _entryAddWindow = new EntryAddWindow();
            _entryAddWindow.Owner = this;
            _entryAddWindow.ShowDialog();
        }

        private void DataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notificationMessage.Notification == MainViewModel.InitAddWindow)
                {
                 
                }
                else if (notificationMessage.Notification == MainViewModel.DismissAddWindow)
                {
                    if (_entryAddWindow != null)
                        _entryAddWindow.Close();
                }
                else if (notificationMessage.Notification == MainViewModel.ShowWcfWarningWindow)
                {
                    MessageBox.Show(string.Format("An error occured while communicating with the server: {0}.\r\nPlease verify that the server is running and whether your connection credentials are valid.", notificationMessage.Sender),
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (notificationMessage.Notification == MainViewModel.CallReceived)
                {
                    var sessionInfo = notificationMessage.Sender as CrmEntry;
                    if (sessionInfo != null)
                    {
                        var incomingCallPopUp = new IncomingCallPopup(sessionInfo);
                        TaskBarIcon.ShowCustomBalloon(incomingCallPopUp, PopupAnimation.Fade, 10000);
                    }
                }
            }));
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = MessageBox.Show("Are you sure you want to exit?", "Call Center CRM", MessageBoxButton.OKCancel);
            if (res != MessageBoxResult.OK)
                e.Cancel = true;
        }

        #region WindowState Events


        private void WindowsIcon_Click(object sender, EventArgs e)
        {
            WindowIconMenu.IsOpen = true;
        }

        private void MaximizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
        }

        private void ChangeViewButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void MinimizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RestoreWindowSize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    MaximizeButton.Visibility = Visibility.Collapsed;
                    ChangeViewButton.Visibility = Visibility.Visible;
                    break;
                case WindowState.Normal:
                    MaximizeButton.Visibility = Visibility.Visible;
                    ChangeViewButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        void DragableGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var mouseX = e.GetPosition(this).X;
            var width = RestoreBounds.Width;
            var x = mouseX - width / 2;

            if (x < 0) x = 0;
            else if (x + width > SystemParameters.PrimaryScreenWidth)
                x = SystemParameters.PrimaryScreenWidth - width;

            WindowState = WindowState.Normal;
            Left = x;
            Top = 0;

            DragMove();
        }

        void DragableGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
                SwitchState();
            else if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void SwitchState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }
        #endregion

        #region Closings
   
        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Mouse Enter and Leave coloring

        private void MinimizeButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void MinizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void MaximizeButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MaximizeButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void MaximizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MaximizeButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void ChangeViewButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ChangeViewButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void ChangeViewButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeViewButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void CloseButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            CloseButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButton.Foreground = new SolidColorBrush(Colors.Gray);
        }
        #endregion
    }
}
