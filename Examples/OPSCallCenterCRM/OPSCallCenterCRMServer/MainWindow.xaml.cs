using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPSCallCenterCRMServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindow_OnStateChanged(null, null);
        }

        #region WindowState Events

        public void ShowWindow()
        {
            Activate();
            ShowInTaskbar = true;
            Show();
            WindowState = WindowState.Normal;
            Focus();
        }

        private void WindowsIcon_Click(object sender, EventArgs e)
        {
            WindowIconMenu.IsOpen = true;
        }

        private void MaximizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
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
        void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

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
