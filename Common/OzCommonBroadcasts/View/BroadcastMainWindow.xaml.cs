using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OzCommon.Utils.DialogService;
using OzCommon.View;
using OzCommon.ViewModel;
using OzCommonBroadcasts.Model;
using OzCommonBroadcasts.ViewModel;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace OzCommonBroadcasts.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BroadcastMainWindow : Window, IWindow
    {
        #region Properties

        WaitWindow waitWindow;
        SettingsWindow settingsWindow;
        AboutWindow aboutWindow;
        DataGridConfig dataGridConfig;
        readonly IBroadcastMainViewModel viewModel;
        System.Reflection.PropertyInfo[] properties;
        IDialogService dialogService;

        #endregion
        
        public BroadcastMainWindow()
            {
            WindowState = WindowState.Maximized ;
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Height + 9;
            
            viewModel = SimpleIoc.Default.GetInstance<IBroadcastMainViewModel>();
            dialogService = SimpleIoc.Default.GetInstance<IDialogService>();

            var entryType = viewModel.GetEntryType();
            properties = entryType.GetProperties();
            InitializeGrid();

            InitializeComponent();

            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            DataContext = viewModel;
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);
            Closed += MainWindow_Closed;
        }

        #region Messaging

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);
        }

        void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                                                  {
                                                      if (notificationMessage.Notification == Messages.ShowWaitWindowLoading)
                                                      {
                                                          if (waitWindow != null) waitWindow.Close();

                                                          waitWindow = new WaitWindow("Loading please wait...")
                                                          {
                                                              Owner = this
                                                          };

                                                          waitWindow.Closed += (sender, e) => viewModel.CancelLoading();

                                                          waitWindow.ShowDialog();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.ShowWaitWindowSaving)
                                                      {
                                                          if (waitWindow != null) waitWindow.Close();

                                                          waitWindow = new WaitWindow("Saving please wait...")
                                                          {
                                                              Owner = this
                                                          };

                                                          waitWindow.Closed += (sender, e) => viewModel.CancelLoading();

                                                          waitWindow.ShowDialog();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.ShowAboutWindow)
                                                      {
                                                          if (aboutWindow != null) aboutWindow.Close();

                                                          aboutWindow = new AboutWindow(viewModel.GetApplicationInformation().ProductName)
                                                                           {
                                                                               DataContext = notificationMessage.Target,
                                                                               Owner = this
                                                                           };
                                                          aboutWindow.ShowDialog();
                                                      }

                                                      else if (notificationMessage.Notification == Messages.ShowSettings)
                                                      {
                                                          if (settingsWindow != null)
                                                              settingsWindow.Close();

                                                          settingsWindow = new SettingsWindow
                                                          {
                                                              DataContext = notificationMessage.Target,
                                                              Owner = this
                                                          };
                                                          settingsWindow.ShowDialog();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.DismissWaitWindow)
                                                      {
                                                          if (waitWindow != null)
                                                          {
                                                              waitWindow.Close();
                                                              waitWindow = null;
                                                          }
                                                      }
                                                      else if (notificationMessage.Notification == Messages.DismissSettingsWindow)
                                                      {
                                                          if (settingsWindow != null)
                                                          {
                                                              settingsWindow.Close();
                                                              settingsWindow = null;
                                                          }
                                                      }
                                                      else if (notificationMessage.Notification == Messages.ReValidateAllRows)
                                                      {
                                                          RevalidateAllRows();
                                                      }
                                                      else if (notificationMessage.Notification == Messages.RefreshDataGridRowNumbers)
                                                      {
                                                          CustumerDataGrid.Items.Refresh();
                                                      }
                                                  }));
        }

        #endregion

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
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Height + 9;
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
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Height + 9;
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RestoreWindowSize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        void BroadCastMainWindow_OnStateChanged(object sender, EventArgs e)
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
                        MaxHeight = Screen.PrimaryScreen.WorkingArea.Height + 9;
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

        void CloseApp()
        {
            var result = dialogService.ShowMessageBox("Are you sure you want to quit?", "Closing",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Close();
        }

        void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            CloseApp();
        }

        void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CloseApp();
        }

        void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            CloseApp();
        }

        #endregion

        #region Mouse Enter and Leave coloring

        void MinimizeButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        void MinizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        void MaximizeButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MaximizeButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        void MaximizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MaximizeButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        void ChangeViewButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ChangeViewButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        void ChangeViewButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeViewButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        void CloseButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            CloseButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        #region DatGrid Events

        void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    CustumerDataGrid.UnselectAllCells();
                    break;
            }
        }

        void CustumerDataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => GridErrorValidation(e.Row)),
                                   System.Windows.Threading.DispatcherPriority.Background);
        }

        void GridErrorValidation(DataGridRow e)
        {
            if (e == null)
                return;

            var dataErrorInfo = e.Item as IDataErrorInfo;

            if (dataErrorInfo == null)
                return;

            foreach (var property in properties)
            {
                var error = dataErrorInfo[property.Name];

                if (error != null)
                {
                    e.HeaderTemplate = (DataTemplate)Resources["errorTemplate"];
                    e.ToolTip = error;
                    break;
                }
                else
                {
                    e.HeaderTemplate = null;
                    e.ToolTip = "";
                }
            }
        }

        void RevalidateAllRows()
        {
            for (int i = 0; i < CustumerDataGrid.Items.Count; ++i)
            {
                var row = CustumerDataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                GridErrorValidation(row);
            }
        }

        void CustumerDataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => GridErrorValidation(e.Row)),
                                   System.Windows.Threading.DispatcherPriority.Background);
        }

        void InitializeGrid()
        {
            dataGridConfig = new DataGridConfig();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);

                foreach (Attribute attribute in attributes)
                {
                    var attributeType = attribute.GetType();

                    if (attributeType == typeof(ReadOnlyPropertyAttribute))
                        dataGridConfig.AddReadOnlyColumnName(property.Name);
                    else if (attributeType == typeof(InvisiblePropertyAttribute))
                        dataGridConfig.AddInvisibleColumnName(property.Name);
                }
            }
        }

        void CustumerDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (dataGridConfig.ReadOnlyColumns.Any(readOnlyColumn => e.Column.Header.Equals(readOnlyColumn)))
                e.Column.IsReadOnly = true;

            if (dataGridConfig.InvisibleColumns.Any(inVisibleColumn => e.Column.Header.Equals(inVisibleColumn)))
                e.Cancel = true;
        }

        void CustumerDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (((DataGridRow)e.Row).Tag != null && ((DataGridRow)e.Row).Tag.ToString() == "ReadOnly")
            {
                e.Cancel = true;
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => GridErrorValidation(e.Row)),
                                       System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "CheckedJobs" || CustumerDataGrid.Items.Count <= 0) return;

            Dispatcher.BeginInvoke(
                new Action(() =>
                                {
                                    var lastElement = ((viewModel.CheckedJobs - 1) < 1) ? 0 : viewModel.CheckedJobs - 1;
                                    if (lastElement >= CustumerDataGrid.Items.Count)
                                    {
                                        lastElement = CustumerDataGrid.Items.Count - 1;
                                    }
                                    CustumerDataGrid.ScrollIntoView(CustumerDataGrid.Items[lastElement]);
                                }));
        }

        void CustumerDataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        #endregion

        void TaskBarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            ShowWindow();
        }


    }
}
